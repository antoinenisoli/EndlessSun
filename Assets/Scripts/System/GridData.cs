using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GridData
{
	public Region ocean = new Region();
	public Color[] debugColors = new Color[] { Color.white };
	public List<Island> islands = new List<Island>();
	public int[,] map;
}
