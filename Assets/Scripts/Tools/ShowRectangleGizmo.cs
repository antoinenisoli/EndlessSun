using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowRectangleGizmo : ShowGizmo
{
    [SerializeField] Vector2 size = Vector2.one;

    public override void Wire()
    {
        Gizmos.DrawWireCube(localCenter, size);
    }

    public override void Solid()
    {
        Gizmos.DrawCube(localCenter, size);
    }

    public void SetSize(Vector2 size)
    {
        this.size = size;
    }
}
