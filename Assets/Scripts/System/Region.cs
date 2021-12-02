using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Region
{
	public List<Vector2Int> CoordinateList;
	public int index;

	public Region(List<Vector2Int> coords, int index)
	{
		CoordinateList = coords;
		this.index = index;
	}

	public void Print()
    {
        foreach (var item in GetCellList())
			MonoBehaviour.print(item.name);
    }

	public List<Cell> GetCellList()
	{
		List<Cell> cellList = new List<Cell>();
		foreach (var item in CoordinateList)
		{
			Cell foundCell = GridManager.Instance.GetCell(item);
			if (foundCell)
				cellList.Add(foundCell);
		}

		return cellList;
	}
}
