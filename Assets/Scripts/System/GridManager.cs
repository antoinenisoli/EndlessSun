using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using Pathfinding;

public class GridManager : MonoBehaviour
{
	public static GridManager Instance;
	[SerializeField] Vector2Int gridSize;
	[SerializeField] RuleTile beachTile, waterTile;
	[SerializeField] Tilemap groundTilemap, propsTilemap;
	GridLayout gridLayout;
	AstarPath astarManager;

	[Header("Generate Map")]
	[SerializeField] CellularAutomata cellularAutomata;
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
		astarManager = FindObjectOfType<AstarPath>();
		Singleton();
		gridLayout = FindObjectOfType<GridLayout>();
		cellularAutomata.Init(gridSize, gridLayout, this);
    }

    public IEnumerator Start()
	{
		GenerateMap();
		yield return new WaitForSeconds(0.01f);
		AssignTypes();
		GenerateProps();
		astarManager.Scan();
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

	void GenerateMap()
	{
        foreach (var item in allCells.Values)
			Destroy(item.gameObject);

		allCells.Clear();
		map = cellularAutomata.RandomFillMap();
		map = cellularAutomata.SmoothMap();
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

	public bool SamplePosition(Vector2 targetPos, int maxDistance, out Cell groundCell)
    {
		float dist = Mathf.Infinity;
		groundCell = null;
		Cell closestCell = ClosestCell(targetPos);

        for (int x = -maxDistance; x < maxDistance + 1; x++)
        {
			for (int y = -maxDistance; y < maxDistance + 1; y++)
			{
				Cell cell = GetCell(closestCell.coordinates + new Vector2Int(x, y));
				if (cell.myType == CellType.Water)
					continue;

				float currentDistance = Vector2.Distance(cell.transform.position, targetPos);
				if (currentDistance < dist)
                {
					dist = currentDistance;
					groundCell = cell;
				}
			}
		}

		return groundCell != null;
    }

	public Vector2 SamplePosition(Vector2 targetPos)
	{
		var constraint = NNConstraint.None;
		constraint.constrainWalkability = true;
		constraint.walkable = true;
		var info = AstarPath.active.GetNearest(targetPos, constraint);
		var closestPoint = info.position;
		print(info.node);
		return closestPoint;
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
		if (!propsTiles)
			return;

		foreach (var item in allCells.Values)
		{
			if (item.myType == CellType.Ground)
			{
				float r = UnityEngine.Random.Range(0, 100);
				if (r < propsProb && !IsBorder(item))
                {
					Vector3Int worldToCell = gridLayout.WorldToCell(new Vector3Int(item.coordinates.x, item.coordinates.y, 0));
					propsTilemap.SetTile(worldToCell, propsTiles);
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

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
			GenerateMap();
	}
}
