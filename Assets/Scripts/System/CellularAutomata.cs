using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class CellularAutomata : MonoBehaviour
{
    [SerializeField] Vector2Int gridSize;
    [SerializeField] GameObject cellPrefab;
    [SerializeField] Sprite[] randomVisual;

    [SerializeField] string seed;
    [SerializeField] int smoothIterations = 5;
    [SerializeField] bool useRandomSeed;
    [SerializeField] [Range(0, 100)] int randomFillPercent = 50;
    int[,] map;
	Dictionary<Vector2Int, Cell> allCells = new Dictionary<Vector2Int, Cell>();

	void OnDrawGizmos()
	{
		if (map == null)
			return;

		for (int x = 0; x < gridSize.x; x++)
			for (int y = 0; y < gridSize.y; y++)
			{
				Gizmos.color = map[x, y] == 1 ? Color.black : Color.white;
				Vector2 pos = new Vector2(-gridSize.x / 2 + x + 0.5f, -gridSize.y / 2 + y + 0.5f);
				Gizmos.DrawCube(pos, Vector3.one);
			}
	}

	void Start()
	{
		GenerateMap();
	}

	void GenerateMap()
	{
		map = new int[gridSize.x, gridSize.y];
		RandomFillMap();

		for (int i = 0; i < smoothIterations; i++)
			SmoothMap();
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
