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
    public Vector3Int tilePosition;

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

    public void Initialize(Vector2Int coord, Vector3Int worldPosition)
    {
        coordinates = coord;
        tilePosition = worldPosition;
        gameObject.name += " " + coord.ToString();
    }

    public void SetType(CellType type)
    {
        myType = type;
    }

    public void GetNeighbours()
    {
        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                if ((x == 0 && y == 0) || !GridManager.Instance.InMapRange(coordinates.x + x, coordinates.y + y))
                    continue;

                Vector2Int coord = new Vector2Int(coordinates.x + x, coordinates.y + y);
                Cell neighbour = GridManager.Instance.GetCell(coord);
                if (neighbour && !neighbours.Contains(neighbour) && neighbour != this)
                    neighbours.Add(neighbour);
            }
        }
    }

    public void SetRegion(int index, Color newColor = default)
    {
        regionIndex = index;
        if (regionDebugVisual)
        {
            regionDebugVisual.gameObject.SetActive(true);
            if (newColor == default)
                regionDebugVisual.color = colors[regionIndex % colors.Length];
            else
                regionDebugVisual.color = newColor;
        }
    }

    public void SetColor(Color c)
    {
        regionDebugVisual.color = c;
    }
}
