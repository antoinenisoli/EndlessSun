using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Region
{
	public string name;
	[HideInInspector] public List<Vector2Int> CoordinateList = new List<Vector2Int>();
	public int index;
	[HideInInspector] public List<Cell> Cells = new List<Cell>();

	public Region()
	{

	}

	public Region(List<Vector2Int> coords, int index) 
	{
		CoordinateList = coords;
		this.index = index;
		name = ToString();
	}

	public void Update()
    {
		name = ToString();
	}

	public bool IsInRegion(Vector2Int coordinates)
    {
        foreach (var item in CoordinateList)
			if (item == coordinates)
				return true;

		return false;
    }

	public void AddCell(Cell newCell)
    {
		Cells.Add(newCell);
		CoordinateList.Add(newCell.coordinates);
	}

	public Vector2 CenterPosition()
    {
		var bound = new Bounds(Cells[0].transform.position, Vector3.zero);
		for (int i = 1; i < Cells.Count; i++)
			bound.Encapsulate(Cells[i].transform.position);

		return bound.center;
	}

	public Vector2 ClosestGroundPos(Vector2 pos)
	{
		float maxDistance = Mathf.Infinity;
		Vector2 newPos = new Vector2();
		foreach (var item in Cells)
		{
			float currentDistance = Vector2.Distance(item.transform.position, pos);
			if (currentDistance < maxDistance && !GridManager.Instance.IsShore(item))
			{
				maxDistance = currentDistance;
				newPos = item.transform.position;
			}
		}

		return newPos;
	}

	public virtual void GetCellList()
	{
		foreach (var item in CoordinateList)
		{
			Cell foundCell = GridManager.Instance.GetCell(item);
			if (foundCell)
				Cells.Add(foundCell);
		}
	}

	public override string ToString()
	{
		return "Region #" + index;
	}
}
