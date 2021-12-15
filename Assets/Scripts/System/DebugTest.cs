using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DebugTest : MonoBehaviour
{
	public LayerMask mask;
	public List<Cell> cells;
	public int step = 50;
	public Cell mainCell;
	public List<Vector2Int> neighbors;
	Dictionary<Vector2Int, Cell> allCells = new Dictionary<Vector2Int, Cell>();

    private void OnDrawGizmosSelected()
    {
		Gizmos.color = Color.green;
		for (int i = 1; i < neighbors.Count; i++)
		{
			Gizmos.DrawLine(new Vector3(neighbors[i - 1].x, neighbors[i - 1].y), new Vector3(neighbors[i].x, neighbors[i].y));
		}
	}

    private void OnValidate()
    {
        if (mainCell)
			transform.position = mainCell.transform.position;
	}

    private void Awake()
    {
		transform.position = mainCell.transform.position;
		cells = FindObjectsOfType<Cell>().ToList();
        foreach (var cell in cells)
        {
			Vector2Int coord = new Vector2Int((int)cell.transform.position.x, (int)cell.transform.position.y);
			allCells.Add(coord, cell);
			cell.coordinates = coord;
		}
	}

    private void Start()
    {
		for (int x = -step; x <= step; x++)
			for (int y = -step; y <= step; y++)
            {				
				Vector2Int neighbor = new Vector2Int(mainCell.coordinates.x + x, mainCell.coordinates.y + y);
				print(new Vector2Int(x,y));
				if (allCells.ContainsKey(neighbor))
				{
					if (!GridManager.Instance.IsColliding(mainCell.coordinates, neighbor, mask))
						neighbors.Add(neighbor);
				}
			}
	}

    public Cell MaxDensityCell(Island island)
	{
		Cell maxDensity = null;
		Dictionary<Cell, List<Vector2Int>> storedNeighbors = new Dictionary<Cell, List<Vector2Int>>();
		foreach (var cell in cells)
		{
			List<Vector2Int> neighbours = new List<Vector2Int>();
			for (int x = -step; x < step; x++)
			{
				for (int y = -step; y < step; y++)
				{
					Vector2Int neighbor = new Vector2Int(cell.coordinates.x + x, cell.coordinates.y + y);
					if (x == 0 && y == 0)
						continue;

					Cell neighborCell = GridManager.Instance.GetCell(neighbor);
					if (neighborCell)
					{
						if (!GridManager.Instance.IsShore(cell) && !GridManager.Instance.IsColliding(cell.coordinates, neighbor, mask))
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
				Debug.DrawLine(new Vector3(coords[i - 1].x, coords[i - 1].y), new Vector3(coords[i].x, coords[i].y), Color.green, 100f);
			}
		}

		return maxDensity;
	}
}
