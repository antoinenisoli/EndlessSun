using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Pathfinding;
using System.IO;
using System.Linq;

public class GridManager : MonoBehaviour
{
	public static GridManager Instance;
	[SerializeField] bool debugMode;
	[SerializeField] Vector2Int gridSize;
	[SerializeField] Tilemap groundTilemap, propsTilemap;
	[SerializeField] RuleTile waterTile;
	[SerializeField] RuleTile[] groundRuleTiles;
	Dictionary<BiomeType, RuleTile> storedGroundTiles = new Dictionary<BiomeType, RuleTile>();
	GridLayout gridLayout;

	[Header("Regions")]
	[SerializeField] Color[] regionColors = new Color[] { Color.white };
	[SerializeField] List<Region> groundRegions = new List<Region>();
	[SerializeField] Transform debugCube;
	int regionSize;

	[Header("Generate Props")]
	[SerializeField] RuleTile propsTiles;
	[SerializeField] float propsProb = 30f;

	[Header("Generate Map")]
	[SerializeField] CellularAutomata cellularAutomata;
    public int[,] map;
	Dictionary<Vector2Int, Cell> allCells = new Dictionary<Vector2Int, Cell>();

    private void OnDrawGizmos()
    {
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(transform.position + new Vector3(gridSize.x/2, gridSize.y/2), new Vector3(gridSize.x, gridSize.y));
    }

    private void Awake()
    {
		gridLayout = FindObjectOfType<GridLayout>();
		Singleton();
		map = new int[gridSize.x, gridSize.y];
		cellularAutomata.Init(gridSize, this);
		StoreRuleTiles();
    }

	public IEnumerator Start()
	{
		if (!debugMode)
		{
			GenerateMap();
			yield return new WaitForSeconds(0.01f);
			GenerateCells();
		}
		else
			DebugMap();

		AssignRegions();
		AssignTypes();
		yield return new WaitForSeconds(0.01f);
		GenerateProps();
		MapText("mapText.txt");
		if (AstarPath.active)
			AstarPath.active.Scan();

		Region r = GridManager.Instance.BiggestRegion();
		Vector2 newPos = r.ClosestGroundPos(r.CenterPosition());
		debugCube.position = newPos;
		GameManager.Player.CheckCollision();
	}

	void Singleton()
	{
		if (Instance == null)
			Instance = this;
	}

	void StoreRuleTiles()
    {
        foreach (var item in groundRuleTiles)
        {
			System.Array array = System.Enum.GetValues(typeof(BiomeType));
			BiomeType randomBiome = (BiomeType)array.GetValue(Random.Range(0, array.Length));
            for (int i = 0; i < array.Length; i++)
            {
				if (item.name.Contains(randomBiome.ToString()) && !storedGroundTiles.ContainsValue(item))
                {
					storedGroundTiles.Add(randomBiome, item);
					break;
                }
            }
		}
    }

	void MapText(string fileName)
    {
		string path = "";
		if (File.Exists(fileName))
			Debug.Log(fileName + " already exists");

		var sr = File.CreateText(path + fileName);
		for (int x = 0; x < gridSize.x; x++)
		{
			for (int y = 0; y < gridSize.y; y++)
			{
				sr.Write(map[x,y]);
			}

			sr.WriteLine();
		}

		sr.Close();
	}

	void DebugMap()
    {
		Cell[] cells = FindObjectsOfType<Cell>();
		foreach (var item in cells)
		{
			Vector2Int newCoord = new Vector2Int((int)item.transform.position.x, (int)item.transform.position.y);
			item.coordinates = newCoord;
			newCoord.x = System.Math.Abs(newCoord.x);
			newCoord.y = System.Math.Abs(newCoord.y);

			if (item.myType == CellType.Ground)
			{
				map[newCoord.x, newCoord.y] = 0;
				groundTilemap.SetTile(new Vector3Int(newCoord.x, newCoord.y, 0), groundRuleTiles[0]);
			}
			else if (item.myType == CellType.Water)
			{
				map[newCoord.x, newCoord.y] = 1;
				groundTilemap.SetTile(new Vector3Int(newCoord.x, newCoord.y, 0), waterTile);
			}

			allCells.Add(newCoord, item);
		}
	}

	public List<Region> GetRegions(int tileType)
	{
		List<Region> regions = new List<Region>();
		int[,] mapFlags = new int[gridSize.x, gridSize.y];

		for (int x = 0; x < gridSize.x; x++)
			for (int y = 0; y < gridSize.y; y++)
				if (mapFlags[x, y] == 0 && map[x, y] == tileType)
				{
					Region newRegion = GetRegionTiles(x, y);
					regions.Add(newRegion);
					foreach (Vector2Int tile in newRegion.CoordinateList)
						mapFlags[tile.x, tile.y] = 1;
				}

		return regions;
	}

	public Region GetRegionTiles(int startX, int startY)
    {
		List<Vector2Int> tiles = new List<Vector2Int>();
		int[,] mapFlags = new int[gridSize.x, gridSize.y];
		int tileType = map[startX, startY];

		Queue<Vector2Int> queue = new Queue<Vector2Int>();
		queue.Enqueue(new Vector2Int(startX, startY));
		mapFlags[startX, startY] = 1;   
		
		while (queue.Count > 0)
        {
			Vector2Int tile = queue.Dequeue();
			tiles.Add(tile);

			for (int x = tile.x - 1; x <= tile.x + 1; x++)
				for (int y = tile.y - 1; y <= tile.y + 1; y++)
					if (InMapRange(x, y) && (y == tile.y || x == tile.x))
						if (mapFlags[x, y] == 0 && map[x, y] == tileType)
						{
							mapFlags[x, y] = 1;
							queue.Enqueue(new Vector2Int(x, y));
						}
		}

		regionSize++;
		Region newRegion = new Region(tiles, regionSize);
		return newRegion;
	}

	public bool InMapRange(int x, int y)
    {
		return x >= 0 && x < gridSize.x && y >= 0 && y < gridSize.y;
	}

	void GenerateMap()
	{
		groundTilemap.ClearAllTiles();
		propsTilemap.ClearAllTiles();
		map = new int[gridSize.x, gridSize.y];
		foreach (var item in allCells.Values)
        {
			if (item)
				Destroy(item.gameObject);
		}

		allCells.Clear();
		cellularAutomata.NewMap();
	}

	public Cell ClosestCell(Vector2 pos)
	{
		float maxDistance = Mathf.Infinity;
		Cell groundCell = null;
        foreach (var item in allCells.Values)
        {
			float currentDistance = Vector2.Distance(item.transform.position, pos);
			if (currentDistance < maxDistance)
			{
				maxDistance = currentDistance;
				groundCell = item;
			}
		}

		return groundCell;
	}

	public bool SamplePosition(Vector2 targetPos, out Vector2 sampledPosition, float maxDistance = 20f)
	{
		var constraint = NNConstraint.Default;
		constraint.constrainWalkability = true;
		constraint.walkable = true;
		var info = AstarPath.active.GetNearest(targetPos, constraint);
		Vector2 closestPoint = info.position;
		float distance = Vector2.Distance(targetPos, closestPoint);

		sampledPosition = new Vector2();
		if (distance < maxDistance)
			sampledPosition = closestPoint;

		return sampledPosition.sqrMagnitude > 0;
	}

	void GenerateCells()
	{
		for (int x = 0; x < gridSize.x; x++)
			for (int y = 0; y < gridSize.y; y++)
			{
				Vector2 pos = new Vector2(x, y);
				Vector3Int worldToCell = gridLayout.WorldToCell(new Vector3Int((int)pos.x, (int)pos.y, 0));
				groundTilemap.SetTile(worldToCell, groundRuleTiles[0]);

				GameObject newCell = groundTilemap.GetInstantiatedObject(worldToCell);
				Cell cellScript = newCell.GetComponent<Cell>();
				Vector2Int coords = new Vector2Int(x, y);
				cellScript.Initialize(coords, worldToCell);
				allCells.Add(coords, cellScript);
			}
	}

	void AssignTypes()
    {
		for (int x = 0; x < map.GetLength(0); x++)
			for (int y = 0; y < map.GetLength(1); y++)
			{
				Vector2Int coords = new Vector2Int(x, y);
				Cell cellScript = GetCell(coords);
				cellScript.GetNeighbours();

				Vector2 pos = new Vector2(x, y);
				Vector3Int worldToCell = gridLayout.WorldToCell(new Vector3Int((int)pos.x, (int)pos.y, 0));

				switch (map[x, y])
                {
                    case 0:
						cellScript.SetType(CellType.Ground);
						groundTilemap.SetTile(worldToCell, groundRuleTiles[0]);
						break;

					case 1:
						cellScript.SetType(CellType.Water);
						groundTilemap.SetTile(worldToCell, waterTile);
						break;
                }
            }
	}

	[ContextMenu("Region random colors")]
	public void NewRegionsColors()
    {
        for (int i = 0; i < regionColors.Length; i++)
			regionColors[i] = GameManager.RandomColor();
    }

	public Region BiggestRegion()
    {
		List<Region> sortedRegions = groundRegions.OrderBy(i => i.Cells.Count).Reverse().ToList();
		return sortedRegions[0];
    }

	public void AssignRegions()
	{
		groundRegions = GetRegions(0);

		for (int i = 0; i < groundRegions.Count; i++)
		{
			Region groundRegion = groundRegions[i];
			Color color = regionColors[i];
			groundRegion.color = color;
			groundRegion.index = i;
			color.a = 0.5f;
			
			foreach (var cell in groundRegion.Cells)
            {
				cell.SetRegion(i, color);
				if (storedGroundTiles.TryGetValue(groundRegion.myBiome, out RuleTile ruleTile))
					groundTilemap.SetTile(cell.tilePosition, ruleTile);
				else if (groundRuleTiles.Length > 0)
					groundTilemap.SetTile(cell.tilePosition, groundRuleTiles[0]);
			}
		}
	}

	public bool IsShore(Cell cell)
    {
        foreach (var item in cell.neighbours)
        {
			if (item.myType == CellType.Water)
				return true;
		}

		return false;
	}

	void GenerateProps()
    {
		if (!propsTiles)
			return;

		foreach (var item in allCells.Values)
		{
			if (item.myType == CellType.Ground)
			{
				float r = Random.Range(0, 100);
				if (r < propsProb && !IsShore(item))
                {
					Vector3Int worldToCell = gridLayout.WorldToCell(new Vector3Int(item.coordinates.x, item.coordinates.y, 0));
					propsTilemap.SetTile(worldToCell, propsTiles);
                }
			}
		}
	}

	public Cell GetCell(Vector2Int coord)
	{
		if (allCells.ContainsKey(coord))
			return allCells[coord];

		return null;
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
			StartCoroutine(Start());
	}
}
