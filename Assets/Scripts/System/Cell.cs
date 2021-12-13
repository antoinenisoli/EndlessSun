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
    public List<Vector2Int> neighbours;
    public Vector3Int tilePosition;

    [Header("Region")]
    public Region myRegion;
    [SerializeField] SpriteRenderer regionDebugVisual;
    [SerializeField] Color[] colors;

    private void OnDrawGizmosSelected()
    {
        foreach (var coord in neighbours)
            Gizmos.DrawLine(transform.position, new Vector2(coord.x, coord.y));
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
                if (!neighbours.Contains(coord) && coord != coordinates)
                    neighbours.Add(coord);
            }
        }
    }

    public void SetRegion(Region region)
    {
        myRegion = region;
    }

    public void SetColor(Color c)
    {
        regionDebugVisual.color = c;
        if (regionDebugVisual)
            regionDebugVisual.gameObject.SetActive(true);
    }
}
