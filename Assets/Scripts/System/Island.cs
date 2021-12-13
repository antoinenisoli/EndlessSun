using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum IslandBiome
{
	Beach,
	Forest,
	Mountain,
	Jungle,
	Volcano,
}

[System.Serializable]
public class Island : Region, IComparable<Island>
{
	public List<Cell> edgeTiles = new List<Cell>();
	public List<Island> connectedIslands = new List<Island>();
	public int IslandSize;
	public bool isAccessibleFromMainIsland;
	public bool isMainIsland;

	public Color color;
	public IslandBiome myBiome;

	public Island()
	{
		
	}

	public Island(List<Vector2Int> coords, int index) : base(coords, index)
	{
		NewBiome();
		IslandSize = coords.Count;
		//GetEdgeTiles();
	}

	public void NewBiome()
	{
		System.Array array = System.Enum.GetValues(typeof(IslandBiome));
		IslandBiome randomBiome = (IslandBiome)array.GetValue(UnityEngine.Random.Range(0, array.Length));
		myBiome = randomBiome;
	}

	public List<Cell> GetEdgeTiles()
    {
		edgeTiles = new List<Cell>();
        foreach (var item in Cells)
			if (GridManager.Instance.IsShore(item) && item.myType != CellType.Water)
				edgeTiles.Add(item);

		return edgeTiles;
    }

    public override void GetCellList()
    {
        base.GetCellList();
		name = ToString() + " " + Cells.Count;
	}

	public void SetAccessibleFromMainIsland()
	{
		if (!isAccessibleFromMainIsland)
		{
			isAccessibleFromMainIsland = true;
			foreach (var connectedIsland in connectedIslands)
				connectedIsland.SetAccessibleFromMainIsland();
		}
	}

	public static void ConnectIslands(Island islandA, Island islandB)
    {
		if (islandA.isAccessibleFromMainIsland)
			islandB.SetAccessibleFromMainIsland();
		else if (islandB.isAccessibleFromMainIsland)
			islandA.SetAccessibleFromMainIsland();

		islandA.connectedIslands.Add(islandB);
		islandB.connectedIslands.Add(islandA);
	}

	public bool IsConnected(Island otherIsland)
    {
		return connectedIslands.Contains(otherIsland);
    }

	public override string ToString()
	{
		return "[" + myBiome + " #" + index + "]";
	}

    public int CompareTo(Island otherIsland)
    {
		return otherIsland.IslandSize.CompareTo(IslandSize);
    }
}
