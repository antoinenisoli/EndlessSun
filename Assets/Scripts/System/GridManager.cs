using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Pathfinding;
using System.IO;
using System.Linq;
using Cinemachine;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
	public static GridManager Instance;
	[SerializeField] bool debugMode;
	[SerializeField] Vector2Int gridSize;
	[SerializeField] Tilemap groundTilemap, propsTilemap;
	[SerializeField] RuleTile waterTile;
	[SerializeField] RuleTile[] groundRuleTiles;
	Dictionary<IslandBiome, RuleTile> storedGroundTiles = new Dictionary<IslandBiome, RuleTile>();
	GridLayout gridLayout;

	[Header("Regions")]
	public Region ocean = new Region();
	[SerializeField] Color[] debugColors = new Color[] { Color.white };
	public List<Island> islands = new List<Island>();
	int regionSize;

	[Header("_Debug")]
	[SerializeField] Text debugText;
	[SerializeField] Transform debugCube;
	int elapsedFrames;
	bool stopFrameCount;

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
		Singleton();

		gridLayout = FindObjectOfType<GridLayout>();
		map = new int[gridSize.x, gridSize.y];
		cellularAutomata.Init(gridSize, this);
		StoreRuleTiles();
    }

	public IEnumerator Start()
	{
		StartCoroutine(DebugCoroutine());
		if (!debugMode)
		{
			GenerateMap();
			yield return new WaitForSeconds(0.01f);
			GenerateCells();
			AssignIslands();
		}
		else
			DebugMap();

		yield return new WaitForSeconds(0.01f);
		SetupMap();
	}

	IEnumerator DebugCoroutine()
    {
		while (!stopFrameCount)
        {
			yield return null;
			elapsedFrames++;
			debugText.text = "Time : " + elapsedFrames + " frames";
		}
	}

	void SetupMap()
    {
		cellularAutomata.CleanSmallIslands();
		foreach (var item in islands)
			item.GetEdgeTiles();

		cellularAutomata.ConnectClosestIslands(islands);
		ProcessMap();
	}

	void ProcessMap()
    {
		DrawGroundTiles();
		DrawWaterTiles();
		CleanUselessCells();

		TeleportPlayer();
		GenerateProps();
		MapText("mapText.txt");
		stopFrameCount = true;
		if (AstarPath.active)
			AstarPath.active.Scan();
	}

	void Singleton()
	{
		if (Instance == null)
			Instance = this;
	}

	void TeleportPlayer()
    {
		Region r = BiggestIsland();
		Vector2 newPos = r.ClosestGroundPos(r.CenterPosition());
		debugCube.position = newPos;
		if (GameManager.Instance)
			GameManager.Player.CheckCollision();
	}

	void StoreRuleTiles()
    {
        foreach (var item in groundRuleTiles)
        {
			System.Array array = System.Enum.GetValues(typeof(IslandBiome));
			IslandBiome randomBiome = (IslandBiome)array.GetValue(Random.Range(0, array.Length));
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
		if (File.Exists(fileName)) Debug.Log(fileName + " file created");

		var sr = File.CreateText(path + fileName);
		for (int x = 0; x < gridSize.x; x++)
		{
			for (int y = 0; y < gridSize.y; y++)
			{
				if (map[x, y] == 1)
					sr.Write("#");
				else
					sr.Write("&");
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

	public List<Island> GetIslands()
	{
		List<Island> convertedIslands = new List<Island>();
        foreach (var item in GetRegions(0))
        {
			Island island = new Island(item.CoordinateList, item.index);
			convertedIslands.Add(island);
        }

		return convertedIslands;
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

	void CleanUselessCells()
	{
		for (int x = 0; x < gridSize.x; x++)
			for (int y = 0; y < gridSize.y; y++)
			{
				Vector2Int coords = new Vector2Int(x, y);
				Vector3Int worldToCell = gridLayout.WorldToCell(new Vector3Int(coords.x, coords.y, 0));
				GameObject uselessCell = groundTilemap.GetInstantiatedObject(worldToCell);
				if (map[coords.x, coords.y] == 1)
                {
					Destroy(uselessCell);
					allCells.Remove(coords);
				}
			}
	}

	void GenerateCells()
	{
		for (int x = 0; x < gridSize.x; x++)
			for (int y = 0; y < gridSize.y; y++)
			{
				Vector2Int coords = new Vector2Int(x, y);
				Vector3Int worldToCell = gridLayout.WorldToCell(new Vector3Int(coords.x, coords.y, 0));
				groundTilemap.SetTile(worldToCell, groundRuleTiles[0]);
				GameObject newCell = groundTilemap.GetInstantiatedObject(worldToCell);

				if (map[coords.x, coords.y] == 1)
					Destroy(newCell);
				else
                {
					Cell cellScript = newCell.GetComponent<Cell>();
					cellScript.Initialize(coords, worldToCell);
					allCells.Add(coords, cellScript);
				}
			}
	}

	void DrawWaterTiles()
    {
		ocean = GetRegions(1)[0];
		foreach (var coord in ocean.CoordinateList)
		{
			Vector3Int worldToCell = gridLayout.WorldToCell(new Vector3Int(coord.x, coord.y, 0));
			groundTilemap.SetTile(worldToCell, waterTile);
		}
	}

	public void DrawGroundTiles()
	{
		foreach (var island in islands)
		{
			foreach (var cell in island.Cells)
			{
				if (storedGroundTiles.TryGetValue(island.myBiome, out RuleTile ruleTile))
					groundTilemap.SetTile(cell.tilePosition, ruleTile);
				else if (groundRuleTiles.Length > 0)
					groundTilemap.SetTile(cell.tilePosition, groundRuleTiles[0]);
			}
		}
	}

	[ContextMenu("Region random colors")]
	public void NewRegionsColors()
    {
		debugColors = new Color[30];
        for (int i = 0; i < debugColors.Length; i++)
			debugColors[i] = GameManager.RandomColor();
    }

	public Island BiggestIsland()
    {
		List<Island> sortedIslands = islands.OrderBy(i => i.Cells.Count).Reverse().ToList();
		return sortedIslands[0];
    }

	public void AssignIslands()
	{
		islands = GetIslands();
		for (int i = 0; i < islands.Count; i++)
		{
			Island island = islands[i];
			Color color = debugColors[i];
			island.color = color;
			island.index = i;
			color.a = 0.5f;
			foreach (var cell in island.Cells)
            {
				cell.GetNeighbours();
				cell.SetRegion(i, color);
			}
		}
	}

	public bool IsShore(Cell cell)
    {
        foreach (var neighbourCoordinates in cell.neighbours)
        {
			if (map[neighbourCoordinates.x, neighbourCoordinates.y] == 1)
				return true;
		}

		return false;
	}

	void GenerateProps()
    {
		if (!propsTiles)
			return;

		foreach (Island island in islands)
            foreach (Cell cell in island.Cells)
            {
				if (!IsShore(cell))
				{
					float r = Random.Range(0, 100);
					if (r < propsProb)
					{
						Vector3Int worldToCell = gridLayout.WorldToCell(new Vector3Int(cell.coordinates.x, cell.coordinates.y, 0));
						propsTilemap.SetTile(worldToCell, propsTiles);
					}
				}
			}
	}

	public Vector3 CellToWorldPoint(Cell cell)
	{
		return new Vector3(-gridSize.x / 2 + 0.5f + cell.coordinates.x, 2 - gridSize.y + 0.5f + cell.coordinates.y);
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
		else if (Input.GetKeyDown(KeyCode.Escape))
			Application.Quit();
	}
}
