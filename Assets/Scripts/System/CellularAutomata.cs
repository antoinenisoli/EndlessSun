using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CellularAutomata
{
	[SerializeField] int seed;
	[SerializeField] int smoothIterations = 5;
	[SerializeField] bool useRandomSeed;
	[SerializeField] [Range(0, 100)] float randomFillPercent = 50;
	[SerializeField] int groundThresholdSize = 50;
    UnityEngine.Vector2Int gridSize;
	GridLayout gridLayout;
	GridManager gridManager;
	int[,] map;

	public void Init(UnityEngine.Vector2Int gridSize, GridLayout gridLayout, GridManager gridManager)
    {
        this.gridSize = gridSize;
        this.gridLayout = gridLayout;
        this.gridManager = gridManager;
    }

	public int[,] NewMap()
    {
		map = RandomFillMap();
		SmoothMap();
		CleanSmallIslands();
		return map;
	}

    public void SmoothMap()
	{
		for (int i = 0; i < smoothIterations; i++)
        {
			for (int x = 0; x < gridSize.x; x++)
				for (int y = 0; y < gridSize.y; y++)
				{
					Vector2 pos = new Vector2(x, y);
					Vector3Int worldToCell = gridLayout.WorldToCell(new Vector3Int((int)pos.x, (int)pos.y, 0));
					int neighbourWallTiles = GetSurroundingWallCount(x, y);
					if (neighbourWallTiles > 4)
						map[x, y] = 1;
					else if (neighbourWallTiles < 4)
						map[x, y] = 0;

					gridManager.SetCellType(map[x, y], worldToCell);
				}
		}
	}

	int GetSurroundingWallCount(int gridX, int gridY)
	{
		int wallCount = 0;
		for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
			for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
				if (gridManager.InMapRange(neighbourX, neighbourY))
				{
					if (neighbourX != gridX || neighbourY != gridY)
						wallCount += map[neighbourX, neighbourY];
				}
				else
					wallCount++;

		return wallCount;
	}

	public int[,] RandomFillMap()
	{
		int[,] map = new int[gridSize.x, gridSize.y];
		int _seed;
		if (useRandomSeed)
			_seed = Random.Range(-5000, 5000);
		else
			_seed = seed; 

		System.Random pseudoRandom = new System.Random(_seed);
		for (int x = 0; x < gridSize.x; x++)
			for (int y = 0; y < gridSize.y; y++)
			{
				Vector2 pos = new Vector2(x, y);
				Vector3Int worldToCell = gridLayout.WorldToCell(new Vector3Int((int)pos.x, (int)pos.y, 0));

				if (x == 0 || x == gridSize.x - 1 || y == 0 || y == gridSize.y - 1)
				{
					map[x, y] = 1;
					gridManager.SetCellType(1, worldToCell);
				}
				else
				{
					int random = pseudoRandom.Next(0, 100);
					if (random < randomFillPercent)
						map[x, y] = 1;
					else
						map[x, y] = 0;

					gridManager.SetCellType(map[x, y], worldToCell);
				}
			}

		return map;
	}

	public void CleanSmallIslands()
	{
		List<Region> waterRegions = gridManager.GetRegions(1);

		int waterThresholdSize = 50;
		foreach (var wallRegion in waterRegions)
			if (wallRegion.CoordinateList.Count < waterThresholdSize)
				foreach (var tile in wallRegion.CoordinateList)
                    map[tile.x, tile.y] = 0;

		List<Region> groundRegions = gridManager.GetRegions(0);
		foreach (Region roomRegion in groundRegions)
			if (roomRegion.CoordinateList.Count < groundThresholdSize)
				foreach (Vector2Int tile in roomRegion.CoordinateList)
					map[tile.x, tile.y] = 1;

		MonoBehaviour.print(groundRegions.Count);
	}

	public void AssignRegions()
    {
		List<Region> groundRegions = gridManager.GetRegions(0);
		foreach (var item in groundRegions)
			item.FillCells(0);

		List<Region> waterRegions = gridManager.GetRegions(1);
		foreach (var item in waterRegions)
			item.FillCells(1);
	}
}
