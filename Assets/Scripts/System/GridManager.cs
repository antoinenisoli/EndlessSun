using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using Pathfinding;

public struct Region
{
	public List<Coord> CoordinateList;

    public Region(List<Coord> coords)
    {
        CoordinateList = coords;
    }
}

public class GridManager : MonoBehaviour
{
	public static GridManager Instance;
	[SerializeField] Vector2Int gridSize;
	[SerializeField] RuleTile beachTile, waterTile;
	[SerializeField] Tilemap groundTilemap, propsTilemap;
	GridLayout gridLayout;

	[Header("Generate Map")]
	[SerializeField] CellularAutomata cellularAutomata;
    public int[,] map;
	Dictionary<Coord, Cell> allCells = new Dictionary<Coord, Cell>();

	[Header("Generate Props")]
	[SerializeField] RuleTile propsTiles;
	[SerializeField] float propsProb = 30f;

    private void OnDrawGizmos()
    {
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(transform.position + new Vector3(gridSize.x/2, gridSize.y/2), new Vector3(gridSize.x, gridSize.y));
    }

    private void Awake()
    {
		Singleton();
		map = new int[gridSize.x, gridSize.y];
		gridLayout = FindObjectOfType<GridLayout>();
		cellularAutomata.Init(gridSize, gridLayout, this);
    }

    public IEnumerator Start()
	{
		GenerateMap();
		yield return new WaitForSeconds(0.01f);
		AssignTypes();
		GenerateProps();
		if (AstarPath.active)
			AstarPath.active.Scan();
	}

	void Singleton()
	{
		if (Instance == null)
			Instance = this;
		else
			Destroy(gameObject);
	}

	public void SetCellType(int index, Vector3Int worldToCell)
    {
        switch (index)
        {
			case 0:
				groundTilemap.SetTile(worldToCell, beachTile);
				break;
			case 1:
				groundTilemap.SetTile(worldToCell, waterTile);
				break;
			default:
                break;
        }
    }

	public List<Region> GetRegions(int tileType)
	{
		List<Region> regions = new List<Region>();
		int[,] mapFlags = new int[gridSize.x, gridSize.y];

		for (int x = 0; x < gridSize.x; x++)
		{
			for (int y = 0; y < gridSize.y; y++)
			{
				if (mapFlags[x, y] == 0 && map[x, y] == tileType)
				{
					Region newRegion = GetRegionTiles(x, y);
					regions.Add(newRegion);

					foreach (Coord tile in newRegion.CoordinateList)
					{
						mapFlags[tile.tileX, tile.tileY] = 1;
					}
				}
			}
		}

		return regions;
	}

	public Region GetRegionTiles(int startX, int startY)
    {
		List<Coord> tiles = new List<Coord>();
		int[,] mapFlags = new int[gridSize.x, gridSize.y];
		int tileType = map[startX, startY];

		Queue<Coord> queue = new Queue<Coord>();
		queue.Enqueue(new Coord(startX, startY));
		mapFlags[startX, startY] = 1;   
		
		while (queue.Count > 0)
        {
			Coord tile = queue.Dequeue();
			tiles.Add(tile);

			for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++)
			{
				for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
				{
					if (InMapRange(x, y) && (y == tile.tileY || x == tile.tileX))
					{
						if (mapFlags[x, y] == 0 && map[x, y] == tileType)
						{
							mapFlags[x, y] = 1;
							queue.Enqueue(new Coord(x, y));
						}
					}
				}
			}
		}

		return new Region(tiles);
	}

	public bool InMapRange(int x, int y)
    {
		return x >= 0 && x < gridSize.x && y >= 0 && y < gridSize.y;
	}

	void GenerateMap()
	{
        foreach (var item in allCells.Values)
			Destroy(item.gameObject);

		allCells.Clear();
		map = cellularAutomata.NewMap();
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

	void AssignTypes()
    {
		for (int x = 0; x < gridSize.x; x++)
			for (int y = 0; y < gridSize.y; y++)
			{
				Vector2 pos = new Vector2(x, y);
				Vector3Int worldToCell = gridLayout.WorldToCell(new Vector3Int((int)pos.x, (int)pos.y, 0));
				GameObject newCell = groundTilemap.GetInstantiatedObject(worldToCell);
				Cell cellScript = newCell.GetComponent<Cell>();

				Coord coords = new Coord(x, y);
				cellScript.Initialize(coords);
				allCells.Add(coords, cellScript);

                switch (map[x, y])
                {
                    case 0:
						cellScript.SetType(CellType.Ground);
						break;

					case 1:
						cellScript.SetType(CellType.Water);
						break;
                }
            }
	}

	bool IsBorder(Cell cell)
    {
        for (int x = -2; x < 2; x++)
        {
            for (int y = -2; y < 2; y++)
            {
				if ((x == 0 && y == 0) || !InMapRange(x, y))
					continue;

				if (map[cell.coordinates.tileX + x, cell.coordinates.tileY + y] == 1)
					return true;
            }
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
				float r = UnityEngine.Random.Range(0, 100);
				if (r < propsProb && !IsBorder(item))
                {
					Vector3Int worldToCell = gridLayout.WorldToCell(new Vector3Int(item.coordinates.tileX, item.coordinates.tileY, 0));
					propsTilemap.SetTile(worldToCell, propsTiles);
                }
			}
		}
	}

	Cell GetCell(int x = 0, int y = 0)
    {
		Coord newCoords = new Coord(x, y);
		return GetCell(newCoords);
    }

	Cell GetCell(Coord coord = default)
	{
		if (allCells.ContainsKey(coord))
			return allCells[coord];

		return null;
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
			GenerateMap();
	}
}
