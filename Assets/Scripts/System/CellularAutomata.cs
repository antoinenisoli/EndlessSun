using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CellularAutomata : MonoBehaviour
{
	public static CellularAutomata Instance;
	[SerializeField] Vector2Int gridSize;
	[SerializeField] RuleTile beachTile, waterTile;
	[SerializeField] Tilemap groundTilemap, propsTilemap;
	GridLayout grid;

	[Header("Generate Map")]
    [SerializeField] string seed;
    [SerializeField] int smoothIterations = 5;
    [SerializeField] bool useRandomSeed;
    [SerializeField] [Range(0, 100)] float randomFillPercent = 50;
    int[,] map;
	Dictionary<Vector2Int, Cell> allCells = new Dictionary<Vector2Int, Cell>();

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
		grid = FindObjectOfType<GridLayout>();
    }

    public IEnumerator Start()
	{
		GenerateMap();
		yield return new WaitForSeconds(0.1f);
		AssignTypes();
		GenerateProps();
	}

	void Singleton()
	{
		if (Instance == null)
			Instance = this;
		else
			Destroy(gameObject);
	}

	void GenerateMap()
	{
		map = new int[gridSize.x, gridSize.y];
        foreach (var item in allCells.Values)
			Destroy(item.gameObject);

		allCells.Clear();
		RandomFillMap();
		for (int i = 0; i < smoothIterations; i++)
			SmoothMap();
	}

	void AssignTypes()
    {
		for (int x = 0; x < gridSize.x; x++)
			for (int y = 0; y < gridSize.y; y++)
			{
				Vector2 pos = new Vector2(x, y);
				Vector3Int worldToCell = grid.WorldToCell(new Vector3Int((int)pos.x, (int)pos.y, 0));
				GameObject newCell = groundTilemap.GetInstantiatedObject(worldToCell);
				Cell cellScript = newCell.GetComponent<Cell>();

				Vector2Int coords = new Vector2Int(x, y);
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
				if (x == 0 || y == 0)
					continue;

				if (map[cell.coordinates.x + x, cell.coordinates.y + y] == 1)
					return true;
            }
        }

		return false;
	}

	void GenerateProps()
    {
		foreach (var item in allCells.Values)
		{
			if (item.myType == CellType.Ground)
			{
				float r = UnityEngine.Random.Range(0, 100);
				if (r < propsProb && !IsBorder(item))
                {
					Vector3Int worldToCell = grid.WorldToCell(new Vector3Int(item.coordinates.x, item.coordinates.y, 0));
					propsTilemap.SetTile(worldToCell, propsTiles);
                }
			}
		}
	}

	void RandomFillMap()
	{
		if (useRandomSeed)
			seed = Time.time.ToString();

		System.Random pseudoRandom = new System.Random(seed.GetHashCode());
		for (int x = 0; x < gridSize.x; x++)
			for (int y = 0; y < gridSize.y; y++)
			{
				Vector2 pos = new Vector2(x, y);
				Vector3Int worldToCell = grid.WorldToCell(new Vector3Int((int)pos.x, (int)pos.y, 0));

				if (x == 0 || x == gridSize.x - 1 || y == 0 || y == gridSize.y - 1)
                {
					map[x, y] = 1;
					groundTilemap.SetTile(worldToCell, waterTile);
				}
				else
                {
					int random = pseudoRandom.Next(0, 100);
					if (random < randomFillPercent)
                    {
						map[x, y] = 1;
						groundTilemap.SetTile(worldToCell, waterTile);
					}
					else
                    {
						map[x, y] = 0;
						groundTilemap.SetTile(worldToCell, beachTile);
					}
				}
			}
	}

	Cell GetCell(int x = 0, int y = 0)
    {
		Vector2Int newCoords = new Vector2Int(x, y);
		return GetCell(newCoords);
    }

	Cell GetCell(Vector2Int coord = default)
	{
		if (allCells.ContainsKey(coord))
			return allCells[coord];

		return null;
	}

	void SmoothMap()
	{
		for (int x = 0; x < gridSize.x; x++)
			for (int y = 0; y < gridSize.y; y++)
			{
				Vector2 pos = new Vector2(x, y);
				Vector3Int worldToCell = grid.WorldToCell(new Vector3Int((int)pos.x, (int)pos.y, 0));
				int neighbourWallTiles = GetSurroundingWallCount(x, y);

				if (neighbourWallTiles > 4)
                {
					map[x, y] = 1;
					groundTilemap.SetTile(worldToCell, waterTile);
				}
				else if (neighbourWallTiles < 4)
                {
					map[x, y] = 0;
					groundTilemap.SetTile(worldToCell, beachTile);
				}
			}
	}

	int GetSurroundingWallCount(int gridX, int gridY)
	{
		int wallCount = 0;
		for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
			for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
				if (neighbourX >= 0 && neighbourX < gridSize.x && neighbourY >= 0 && neighbourY < gridSize.y)
				{
					if (neighbourX != gridX || neighbourY != gridY)
						wallCount += map[neighbourX, neighbourY];
				}
				else
					wallCount++;

		return wallCount;
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
			GenerateMap();
	}
}
