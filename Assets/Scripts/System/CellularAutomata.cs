using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellularAutomata : MonoBehaviour
{
	MapInfo mainMap => gridManager.MainMap;
	[SerializeField] int seed;
	[SerializeField] int smoothIterations = 5;
	[SerializeField] bool useRandomSeed = true;
	[SerializeField] [Range(0, 100)] float randomFillPercent = 51;

	[Header("Clean threshold")]
	[SerializeField] int groundThresholdSize = 70;
	[SerializeField] int waterThresholdSize = 20;
    Vector2Int gridSize;
	GridManager gridManager;

	[Header("Bridges")]
	public bool generateBridges = true;
	public int bridgeProb = 50;
	[SerializeField] int lineRadius = 1;
	[SerializeField] List<Region> bridges = new List<Region>();

	public void Init(Vector2Int gridSize, GridManager gridManager)
    {
        this.gridSize = gridSize;
        this.gridManager = gridManager;
    }

	public void NewMap()
    {
		RandomFillMap();
		SmoothMap();
		CleanMap();
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
						mainMap.map[x, y] = 1;
					else if (neighbourWallTiles < 4)
						mainMap.map[x, y] = 0;
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
						wallCount += mainMap.map[neighbourX, neighbourY];
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
					mainMap.map[x, y] = 1;
				}
				else
				{
					int random = pseudoRandom.Next(0, 100);
					if (random < randomFillPercent)
						mainMap.map[x, y] = 1;
					else
						mainMap.map[x, y] = 0;
				}
			}
	}

	public void CleanMap()
	{
		mainMap.seas = gridManager.GetRegions(1);
		RemoveSmallSeas();
		mainMap.islands = gridManager.GetIslands();
		RemoveSmallIslands();
	}

	public void RemoveSmallSeas()
	{
		List<Region> smallSeas = new List<Region>();
		foreach (var sea in mainMap.seas)
			if (sea.CoordinateList.Count < waterThresholdSize)
				smallSeas.Add(sea);

        foreach (var sea in smallSeas)
        {
			foreach (var tile in sea.CoordinateList)
				mainMap.map[tile.x, tile.y] = 0;

			if (mainMap.seas.Contains(sea))
				mainMap.seas.Remove(sea);
		}
	}

	public void RemoveSmallIslands()
    {
		List<Island> smallIslands = new List<Island>();
		foreach (Island island in mainMap.islands)
			if (island.CoordinateList.Count < groundThresholdSize)
				smallIslands.Add(island);

        foreach (var smallIsland in smallIslands)
        {
			foreach (var coord in smallIsland.CoordinateList)
            {
				mainMap.map[coord.x, coord.y] = 1;
				Region ocean = gridManager.BiggestRegion(mainMap.seas);
				ocean.CoordinateList.Add(coord);
			}

			mainMap.islands.Remove(smallIsland);
        }

		mainMap.islands.Sort();
		if (mainMap.islands.Count > 0)
        {
			mainMap.islands[0].isMainIsland = true;
			mainMap.islands[0].isAccessibleFromMainIsland = true;
		}
	}

	public bool CheckBridgeProb()
    {
		int random = Random.Range(0, 100);
		bool result = random < bridgeProb;
		return result;
    }

	public void ConnectClosestIslands(List<Island> allIslands, bool forceAccessibilityFromMainIsland = false)
    {
		if (!generateBridges)
			return;

		List<Island> islandListA = new List<Island>();
		List<Island> islandListB = new List<Island>();
		if (forceAccessibilityFromMainIsland)
        {
            foreach (var island in allIslands)
            {
				if (island.isAccessibleFromMainIsland)
					islandListB.Add(island);
				else
					islandListA.Add(island);
            }
        }
		else
        {
			islandListA = allIslands;
			islandListB = allIslands;
        }

		int bestDistance = 0;
		Cell bestCellA = null;
		Cell bestCellB = null;
		Island bestIslandA = new Island();
		Island bestIslandB = new Island();
		bool possibleConnectionFound = false;

		foreach (var islandA in islandListA)
        {
			if (!forceAccessibilityFromMainIsland)
            {
				possibleConnectionFound = false;
				if (islandA.connectedIslands.Count > 0)
					continue;
            }

            foreach (var islandB in islandListB)
            {
				if (islandA == islandB || islandA.IsConnected(islandB))
					continue;

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

			if (possibleConnectionFound && !forceAccessibilityFromMainIsland && CheckBridgeProb())
				CreatePassage(bestIslandA, bestIslandB, bestCellA, bestCellB);
		}

		if (possibleConnectionFound && forceAccessibilityFromMainIsland)
        {
			if (CheckBridgeProb())
				CreatePassage(bestIslandA, bestIslandB, bestCellA, bestCellB);

			ConnectClosestIslands(allIslands, true);
		}

		if (!forceAccessibilityFromMainIsland)
			ConnectClosestIslands(allIslands, true);
    }

	List<Vector2Int> GetLine(Vector2Int from, Vector2Int to)
    {
		List<Vector2Int> line = new List<Vector2Int>();
		int x = from.x;
		int y = from.y;

		int dx = to.x - from.x;
		int dy = to.y - from.y;

		bool inverted = false;
		int step = System.Math.Sign(dx);
		int gradientStep = System.Math.Sign(dy);
		int longest = Mathf.Abs(dx);
		int shortest = Mathf.Abs(dy);
		if (longest < shortest)
        {
			inverted = true;
			longest = Mathf.Abs(dy);
			shortest = Mathf.Abs(dx);
			step = System.Math.Sign(dy);
			gradientStep = System.Math.Sign(dx);
		}

		int gradientAccumulation = longest / 2;
        for (int i = 0; i < longest; i++)
        {
			line.Add(new Vector2Int(x, y));
			if (inverted)
				y += step;
			else
				x += step;

			gradientAccumulation += shortest;
			if (gradientAccumulation >= longest)
            {
				if (inverted)
					x += gradientStep;
				else
					y += gradientStep;

				gradientAccumulation -= longest;
            }
        }

		return line;
    }

	void CreatePassage(Island islandA, Island islandB, Cell cellA, Cell cellB)
    {
		//Debug.Log("make passage between " + islandA + " (" + cellA + ")" + " and " + islandB + " (" + cellB + ")");
		Island.ConnectIslands(islandA, islandB);
		Debug.DrawLine(cellA.transform.position, cellB.transform.position, Color.red, 100f);
		List<Vector2Int> lines = GetLine(cellA.coordinates, cellB.coordinates);

		Region bridge = new Region();
		bridge.CoordinateList.AddRange(lines);
		bridges.Add(bridge);
        foreach (var line in lines)
			LineToCells(line, lineRadius);
    }

	void LineToCells(Vector2Int coord, int radius)
    {
        for (int x = -radius; x <= radius; x++)
			for (int y = -radius; y <= radius; y++)
				if (x*x + y*y <= radius*radius)
                {
					int realX = coord.x + x;
					int realY = coord.y + y;

					Vector2Int c = new Vector2Int(realX, realY);
					if (gridManager.InMapRange(c.x, c.y))
                    {
						mainMap.map[c.x, c.y] = 2;
                        foreach (var sea in mainMap.seas)
							if (sea.CoordinateList.Contains(c))
								sea.CoordinateList.Remove(c);
					}
                }
	}
}
