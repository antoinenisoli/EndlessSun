using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class CellularAutomata : MonoBehaviour
{
    [SerializeField] Vector2Int gridSize;
    [SerializeField] GameObject cellPrefab;
    [SerializeField] Sprite[] groundSprites;

    [SerializeField] string seed;
    [SerializeField] int smoothIterations = 5;
    [SerializeField] bool useRandomSeed;
    [SerializeField] [Range(0, 100)] float randomFillPercent = 50;
    int[,] map;
	Dictionary<Vector2Int, Cell> allCells = new Dictionary<Vector2Int, Cell>();

	void Start()
	{
		GenerateMap();
		AssignSprites();
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

	string WriteMatrix(bool[,] matrix)
    {
		string output = " ";
		for (int x = 0; x <= matrix.GetLength(0) - 1; x++)
			for (int y = 0; y <= matrix.GetLength(1) - 1; y++)
				output += " " + matrix[x, y].GetHashCode() + " ";

		return output;
	}

	bool[,] GetNeighboorIndex(Cell cell)
    {
		bool[,] neighbourIndex = new bool[3,3];

		for (int y = -1; y <= 1; y++)
		{
			for (int x = -1; x <= 1; x++)
			{
				bool correct = (x != 0 || y != 0) 
					&& cell.coordinates.x + x >= 0 
					&& cell.coordinates.x + x < gridSize.x 
					&& cell.coordinates.y + y >= 0 
					&& cell.coordinates.y + y < gridSize.y;

				if (correct)
				{
					if (map[cell.coordinates.x + x, cell.coordinates.y + y] == 0)
                    {
						neighbourIndex[x + 1, y + 1] = true;
						cell.neighbours.Add(GetCell(cell.coordinates.x + x, cell.coordinates.y + y));
					}
				}
			}
		}

		return neighbourIndex;
	}

	void AssignSprites()
    {
		foreach (var item in allCells.Values)
        {
			if (map[item.coordinates.x, item.coordinates.y] == 0)
			{
				bool[,] index = GetNeighboorIndex(item);
				if (item.neighbours.Count >= 8)
					continue;
				else
                {
					print(item.coordinates + WriteMatrix(index));
					if (index[1, 2] && index[1,0])
                    {
						item.SetSprite(groundSprites[1]);
					}
				}
			}
			else
				continue;
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
				Vector2 pos = new Vector2(-gridSize.x / 2 + x + 0.5f, -gridSize.y / 2 + y + 0.5f);
				GameObject newCell = Instantiate(cellPrefab, pos, Quaternion.identity, transform);
				Cell cellScript = newCell.GetComponent<Cell>();
				Vector2Int coordinates = new Vector2Int(x, y);
				allCells.Add(coordinates, cellScript);
				cellScript.Initialize(coordinates);

				if (x == 0 || x == gridSize.x - 1 || y == 0 || y == gridSize.y - 1)
                {
					map[x, y] = 1;
					cellScript.SetType(CellType.Water);
				}
				else
                {
					int random = pseudoRandom.Next(0, 100);
					if (random < randomFillPercent)
                    {
						map[x, y] = 1;
						cellScript.SetType(CellType.Water);
					}
					else
                    {
						map[x, y] = 0;
						cellScript.SetType(CellType.Ground);
					}
				}
			}
	}

	Cell GetCell(int x = 0, int y = 0, Vector2Int coord = default)
    {
		if (allCells.ContainsKey(coord) && coord != default)
			return allCells[coord];
		else
        {
			Vector2Int newCoords = new Vector2Int(x, y);
			if (allCells.ContainsKey(newCoords))
				return allCells[newCoords];
		}

		return null;
    }

	void SmoothMap()
	{
		for (int x = 0; x < gridSize.x; x++)
			for (int y = 0; y < gridSize.y; y++)
			{
				int neighbourWallTiles = GetSurroundingWallCount(x, y);
				if (neighbourWallTiles > 4)
                {
					map[x, y] = 1;
					GetCell(x, y).SetType(CellType.Water);
				}
				else if (neighbourWallTiles < 4)
                {
					map[x, y] = 0;
					GetCell(x, y).SetType(CellType.Ground);
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
