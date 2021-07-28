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
    [SerializeField] Sprite[] sprites;

    SpriteRenderer sprRenderer;
    BoxCollider2D coll;

    private void Awake()
    {
        coll = GetComponent<BoxCollider2D>();
        sprRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void SetType(CellType type)
    {
        myType = type;
        sprRenderer.sprite = sprites[(int)myType];
        coll.enabled = myType == CellType.Water;
    }
}
