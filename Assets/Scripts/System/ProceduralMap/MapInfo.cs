using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class MapInfo
{
	[HideInInspector] public List<Region> seas = new List<Region>();
	public List<Island> islands = new List<Island>();
	public int regionSize;
	public int[,] map;
    public int[,] biomeFlags;
    public Dictionary<Vector2Int, Cell> allCells = new Dictionary<Vector2Int, Cell>();

    public MapInfo(int[,] map)
    {
        this.map = map;
        allCells = new Dictionary<Vector2Int, Cell>();
    }

    public MapInfo(int[,] map, List<Region> seas, List<Island> islands, int regionSize, int[,] biomeFlags, Dictionary<Vector2Int, Cell> allCells)
    {
        this.seas = seas;
        this.islands = islands;
        this.regionSize = regionSize;
        this.map = map;
        this.allCells = allCells;
        this.biomeFlags = biomeFlags;
    }

    public string ToText()
    {
        string text = "";
        for (int x = 0; x < biomeFlags.GetLength(0); x++)
        {
            for (int y = 0; y < biomeFlags.GetLength(1); y++)
            {
                if (biomeFlags[x, y] == -1)
                    text += "#";
                else if (biomeFlags[x, y] == 0)
                    text += "_";
                else
                    text += biomeFlags[x, y];
            }

            text += "\n";
        }

        return text;
    }
}
