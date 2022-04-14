using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowSphereGizmo : ShowGizmo
{
    [SerializeField] float radius = 0.1f;

    public override void Wire()
    {
        Gizmos.DrawWireSphere(localCenter, radius);
    }

    public override void Solid()
    {
        Gizmos.DrawSphere(localCenter, radius);
    }

    public void SetSize(float radius)
    {
        this.radius = radius;
    }
}
