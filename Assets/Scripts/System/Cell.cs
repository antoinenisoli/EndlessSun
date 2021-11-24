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
    public Coord coordinates;
    public List<Cell> neighbours;

    private void OnDrawGizmosSelected()
    {
        foreach (var item in neighbours)
        {
            Gizmos.DrawLine(transform.position, item.transform.position);
        }
    }

    public void Initialize(Coord coord)
    {
        coordinates = coord;
        gameObject.name += " " + coord;
    }

    public void SetType(CellType type)
    {
        myType = type;
    }
}
