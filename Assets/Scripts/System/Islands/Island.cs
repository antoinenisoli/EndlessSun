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
	[HideInInspector] public List<Cell> edgeTiles = new List<Cell>();
	public List<Island> connectedIslands = new List<Island>();
	List<EnemySpawner> spawners = new List<EnemySpawner>();
	public int IslandSize;
	public bool isAccessibleFromMainIsland;
	public bool isMainIsland;
	public IslandProfile profile;
	public IslandBiome myBiome;

	public Island()
	{
		
	}

	public Island(List<Vector2Int> coords, int index) : base(coords, index)
	{
		IslandSize = coords.Count;
		myBiome = RandomBiome();
	}

	public void SetProfile(IslandProfile profile, IslandBiome biome, int index)
    {
		this.profile = profile;
		myBiome = biome;
		this.index = index;
		name = ToString() + " " + Cells.Count;
	}

	public static IslandBiome RandomBiome()
	{
		return GameDevHelper.RandomEnum<IslandBiome>();
	}

	public List<Cell> GetEdgeTiles()
    {
		edgeTiles = new List<Cell>();
        foreach (var item in Cells)
			if (GridManager.Instance.IsShore(item) && item.myType != CellType.Water)
				edgeTiles.Add(item);

		return edgeTiles;
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

	public void SpawnEnemies()
    {
		for (int i = 0; i < Cells.Count; i += 50)
		{
			Cell cell = Cells[i];
			float r = UnityEngine.Random.Range(0, 100);
			if (r < profile.spawnerProb)
			{
				Vector3Int worldToCell = new Vector3Int(cell.coordinates.x, cell.coordinates.y, 0);
				GameObject spawnerObject = UnityEngine.Object.Instantiate(profile.enemySpawner, worldToCell, Quaternion.identity, cell.transform);
				EnemySpawner spawnerScript = spawnerObject.GetComponent<EnemySpawner>();
				spawnerScript.Spawn(profile.spawnData);
				spawners.Add(spawnerScript);
			}
		}
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
