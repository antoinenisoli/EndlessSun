using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CellType
{
    Ground,
    Water,
}

public class Cell : MonoBehaviour
{
    public CellType myType;
    public Vector2Int coordinates;
    public List<Cell> neighbours;

    [Header("Region")]
    public int regionIndex = 0;
    [SerializeField] SpriteRenderer regionDebugVisual;
    [SerializeField] Color[] colors;

    private void OnDrawGizmosSelected()
    {
        foreach (var item in neighbours)
        {
            Gizmos.DrawLine(transform.position, item.transform.position);
        }
    }

    public void Initialize(Vector2Int coord)
    {
        coordinates = coord;
        gameObject.name += " " + coord.ToString();
    }

    public void SetType(CellType type)
    {
        myType = type;
    }

    public void SetRegion(int index)
    {
        //print("fill " + index);
        regionIndex = index;
        regionDebugVisual.color = colors[regionIndex % colors.Length];
    }

    public override string ToString()
    {
        return base.ToString() + " " + coordinates.ToString();
    }
}
