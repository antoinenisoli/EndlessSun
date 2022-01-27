using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using System.IO;
using Pathfinding;
using System.Linq;
using UnityEditor.Tilemaps;

public class GridManager : MonoBehaviour 
{
	public static GridManager Instance;
	public Tilemap groundTilemap, propsTilemap;
	public BoundsInt mapBounds;
	public GameObject cellPrefab;
	public Transform cellParent;

	[Header("Regions")]
	[SerializeField] LayerMask tilemapMask;
	public MapInfo MainMap;
	MapGenerator mapGenerator;
	GridLayout gridLayout;

	void Awake()
	{
		if (!Instance)
			Instance = this;

		gridLayout = FindObjectOfType<GridLayout>();
		mapGenerator = GetComponentInChildren<MapGenerator>();
		GenerateMap();
	}

	public Cell CreateCell(Vector2Int coords, Vector2 position)
    {
		if (!MainMap.allCells.ContainsKey(coords))
		{
			GameObject cellGameobject = Instantiate(cellPrefab, position, Quaternion.identity, cellParent);
			Cell cellScript = cellGameobject.GetComponent<Cell>();
			cellScript.Initialize(coords, default);
			cellScript.GetNeighbours();
			if (string.IsNullOrEmpty(cellScript.myRegion.name))
				cellScript.SetRegion(new Region(null, -1));

			MainMap.allCells.Add(coords, cellScript);
			return cellScript;
		}

		return null;
	}

	void BuildMapFromTilemap()
    {
		BoundsInt bounds = groundTilemap.cellBounds;
		MainMap = new MapInfo(new int[bounds.size.x, bounds.size.y]);
		mapBounds = bounds;

		foreach (var pos in bounds.allPositionsWithin)
		{
			Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
			Vector2Int coords = new Vector2Int(localPlace.x, localPlace.y);
			Vector3 place = groundTilemap.CellToWorld(localPlace);
			if (groundTilemap.HasTile(localPlace))
			{
				if (groundTilemap.GetColliderType(localPlace) == Tile.ColliderType.None)
                {
					Cell cell = CreateCell(coords, place);
					cell.SetType(CellType.Ground);
				}
			}
		}
	}

	public void GenerateMap()
    {
		MainMap = new MapInfo(null);
		if (mapGenerator && mapGenerator.gameObject.activeSelf)
			mapGenerator.StartNewMap();		
		else if (groundTilemap)
			BuildMapFromTilemap();
	}

	public Island BiggestIsland(List<Island> specificIslands = null)
	{
		List<Island> sortedIslands = specificIslands;
		if (specificIslands == null)
			sortedIslands = MainMap.islands;

		sortedIslands.Sort();
		if (sortedIslands.Count > 0)
			return sortedIslands[0];
		else
			return null;
	}

	public MapInfo GetMap()
	{
		return new MapInfo(MainMap.map, MainMap.seas, MainMap.islands, MainMap.regionSize, MainMap.biomeFlags, MainMap.allCells);
	}

	public Region BiggestRegion(List<Region> regions)
	{
		List<Region> sortedRegions = regions.OrderBy(i => i.CoordinateList.Count).Reverse().ToList();
		return sortedRegions[0];
	}

	public bool IsShore(Cell cell)
	{
		foreach (var neighbourCoordinates in cell.neighbours)
			if (MainMap.map[neighbourCoordinates.x, neighbourCoordinates.y] == 1)
				return true;

		return false;
	}

	public void TeleportPlayer()
	{
		if (GameManager.Instance)
			GameManager.Player.CheckCollision();
	}

	public static void MapText(string fileName, MapInfo mapInfo)
	{
		string path = "";
		if (File.Exists(fileName)) Debug.Log(fileName + " file created");
		var sr = File.CreateText(path + fileName);
		sr.Write(mapInfo.ToText());
		sr.Close();
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
		int[,] mapFlags = new int[MainMap.map.GetLength(0), MainMap.map.GetLength(1)];

		for (int x = 0; x < MainMap.map.GetLength(0); x++)
			for (int y = 0; y < MainMap.map.GetLength(1); y++)
				if (mapFlags[x, y] == 0 && MainMap.map[x, y] == tileType)
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
		int[,] mapFlags = new int[MainMap.map.GetLength(0), MainMap.map.GetLength(1)];
		int tileType = MainMap.map[startX, startY];

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
						if (mapFlags[x, y] == 0 && MainMap.map[x, y] == tileType)
						{
							mapFlags[x, y] = 1;
							queue.Enqueue(new Vector2Int(x, y));
						}
		}

		MainMap.regionSize++;
		Region newRegion = new Region(tiles, MainMap.regionSize);
		return newRegion;
	}

	public bool InMapRange(int x, int y)
	{
		return x >= 0 && x < MainMap.map.GetLength(0) && y >= 0 && y < MainMap.map.GetLength(1);
	}

	public Cell ClosestCell(Vector2 pos)
	{
		float maxDistance = Mathf.Infinity;
		Cell groundCell = null;
		foreach (var item in MainMap.allCells.Values)
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

	public Vector3Int ToWorldPosition(Vector2Int coords)
    {
		return gridLayout.WorldToCell(new Vector3Int(coords.x, coords.y, 0));
	}

	public Cell GetCell(Vector2Int coord)
	{
		if (MainMap.allCells.TryGetValue(coord, out Cell cell))
			return cell;

		return null;
	}

	public Cell MaxDensityCell(Island island)
    {
		Cell maxDensity = null;
		Dictionary<Cell, List<Vector2Int>> storedNeighbors = new Dictionary<Cell, List<Vector2Int>>();
        foreach (var cell in island.Cells)
        {
			List<Vector2Int> neighbours = new List<Vector2Int>();
			int step = 50;
            for (int x = -step; x < step; x++)
            {
                for (int y = -step; y < step; y++)
                {
					Vector2Int neighbor = new Vector2Int(cell.coordinates.x + x, cell.coordinates.y + y);
					if (x == 0 && y == 0)
						continue;

					Cell neighborCell = GetCell(neighbor);
					if (neighborCell)
                    {
						if (!IsShore(cell) && !IsColliding(cell.coordinates, neighbor, tilemapMask))
							neighbours.Add(neighbor);
					}
				}
            }

			if (neighbours.Count > 0)
				storedNeighbors.Add(cell, neighbours);
        }

		if (storedNeighbors.Values.Count > 0)
		{
			int maxCount = 0;
			foreach (var item in storedNeighbors)
				if (item.Value.Count >= maxCount)
				{
					maxDensity = item.Key;
					maxCount = item.Value.Count;
				}

			print(maxDensity + " " + maxCount);
            for (int i = 1; i < storedNeighbors[maxDensity].Count; i++)
            {
				List<Vector2Int> coords = storedNeighbors[maxDensity];
				//Debug.DrawLine(new Vector3(coords[i - 1].x, coords[i - 1].y), new Vector3(coords[i].x, coords[i].y), Color.green, 100f);
            }
		}

		return maxDensity;
	}

	public Vector2 ClosestWalkable(Vector2 targetPos)
	{
		var constraint = NNConstraint.Default;
		constraint.constrainWalkability = true;
		constraint.walkable = true;
		var info = AstarPath.active.GetNearest(targetPos, constraint);
		Vector2 closestPoint = info.position;
		return closestPoint;
	}

	public bool SamplePosition(Vector2 targetPos, float minDistance, out Vector2 sampledPos)
	{
		sampledPos = ClosestWalkable(targetPos);
		Vector2Int gridPos = GameDevHelper.ToVector2Int(sampledPos);
		print(GetCell(gridPos));
		float d = Vector2.Distance(sampledPos, targetPos);
		return d < minDistance;
	}

	public bool IsColliding(Vector2Int cellA, Vector2Int cellB, LayerMask layerMask)
    {
		bool ray = Physics2D.Linecast(cellA, cellB, layerMask);
		return ray;
    }
}
