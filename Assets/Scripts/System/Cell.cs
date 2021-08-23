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
    [SerializeField] Sprite[] sprites;

    SpriteRenderer sprRenderer;
    BoxCollider2D coll;

    private void OnDrawGizmosSelected()
    {
        foreach (var item in neighbours)
        {
            Gizmos.DrawLine(transform.position, item.transform.position);
        }
    }

    private void Awake()
    {
        coll = GetComponent<BoxCollider2D>();
        sprRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void Initialize(Vector2Int coord)
    {
        this.coordinates = coord;
        gameObject.name += " " + coord;
    }

    public void SetSprite(Sprite spr)
    {
        sprRenderer.sprite = spr;
    }

    public void SetType(CellType type)
    {
        myType = type;
        sprRenderer.sprite = sprites[(int)myType];
        coll.enabled = myType == CellType.Water;
    }
}
