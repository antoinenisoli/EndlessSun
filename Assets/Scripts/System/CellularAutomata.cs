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
	[SerializeField] int groundThresholdSize = 50, waterThresholdSize = 50;
    Vector2Int gridSize;
	GridManager gridManager;

	public void Init(Vector2Int gridSize, GridManager gridManager)
    {
        this.gridSize = gridSize;
        this.gridManager = gridManager;
    }

	public void NewMap()
    {
		RandomFillMap();
		SmoothMap();
		CleanSmallIslands();
	}

    public void SmoothMap()
	{
		for (int i = 0; i < smoothIterations; i++)
        {
			for (int x = 0; x < gridSize.x; x++)
				for (int y = 0; y < gridSize.y; y++)
				{
					int neighbourWallTiles = GetSurroundingWallCount(x, y);
					if (neighbourWallTiles > 4)
						gridManager.map[x, y] = 1;
					else if (neighbourWallTiles < 4)
						gridManager.map[x, y] = 0;
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
						wallCount += gridManager.map[neighbourX, neighbourY];
				}
				else
					wallCount++;

		return wallCount;
	}

	public void RandomFillMap()
	{
		if (useRandomSeed)
			seed = Random.Range(-5000, 5000);

		System.Random pseudoRandom = new System.Random(seed);
		for (int x = 0; x < gridSize.x; x++)
			for (int y = 0; y < gridSize.y; y++)
			{
				if (x == 0 || x == gridSize.x - 1 || y == 0 || y == gridSize.y - 1)
				{
					gridManager.map[x, y] = 1;
				}
				else
				{
					int random = pseudoRandom.Next(0, 100);
					if (random < randomFillPercent)
						gridManager.map[x, y] = 1;
					else
						gridManager.map[x, y] = 0;
				}
			}
	}

	public void CleanSmallIslands()
	{
		List<Region> waterRegions = gridManager.GetRegions(1);
		foreach (var wallRegion in waterRegions)
			if (wallRegion.CoordinateList.Count < waterThresholdSize)
				foreach (var tile in wallRegion.CoordinateList)
					gridManager.map[tile.x, tile.y] = 0;

		List<Island> allIslands = gridManager.GetIslands();
		List<Island> survivingIslands = new List<Island>();
		foreach (Island island in allIslands)
			if (island.CoordinateList.Count < groundThresholdSize)
            {
				foreach (Vector2Int tile in island.CoordinateList)
					gridManager.map[tile.x, tile.y] = 1;
			}
			else
				survivingIslands.Add(new Island(island.CoordinateList, island.index));

        foreach (var item in allIslands) item.GetEdgeTiles();
		foreach (var item in survivingIslands) item.GetEdgeTiles();
		ConnectClosestIslands(survivingIslands);
	}

	void ConnectClosestIslands(List<Island> allIslands)
    {
		int bestDistance = 0;
		Cell bestCellA = null;
		Cell bestCellB = null;
		Island bestIslandA = new Island();
		Island bestIslandB = new Island();
		bool possibleConnectionFound = false;

		foreach (var islandA in allIslands)
        {
			possibleConnectionFound = false;
            foreach (var islandB in allIslands)
            {
				if (islandA == islandB)
					continue;
				if (islandA.IsConnected(islandB))
                {
					possibleConnectionFound = false;
					break;
				}

                for (int indexA = 0; indexA < islandA.edgeTiles.Count; indexA++)
                {
					for (int indexB = 0; indexB < islandB.edgeTiles.Count; indexB++)
					{
						Cell cellA = islandA.edgeTiles[indexA];
						Cell cellB = islandB.edgeTiles[indexB];
						float distanceX = Mathf.Pow(cellA.coordinates.x - cellB.coordinates.x, 2);
						float distanceY = Mathf.Pow(cellA.coordinates.y - cellB.coordinates.y, 2);
						int distanceBetweenIsland = (int)distanceX + (int)distanceY;

						if (distanceBetweenIsland < bestDistance || !possibleConnectionFound)
                        {
							bestDistance = distanceBetweenIsland;
							possibleConnectionFound = true;

							bestCellA = cellA;
							bestCellB = cellB;

							bestIslandA = islandA;
							bestIslandB = islandB;
                        }
					}
				}
            }
        }

		if (possibleConnectionFound)
			CreatePassage(bestIslandA, bestIslandB, bestCellA, bestCellB);
    }

	void CreatePassage(Island islandA, Island islandB, Cell cellA, Cell cellB)
    {
		Debug.Log("make passage between " + islandA + " and " + islandB);
		Island.ConnectIslands(islandA, islandB);
		Debug.DrawLine(gridManager.CellToWorldPoint(cellA), gridManager.CellToWorldPoint(cellB), Color.green, 100f);
    }
}
