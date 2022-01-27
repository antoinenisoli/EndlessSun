using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShowGizmo : MonoBehaviour
{
    [SerializeField] protected bool alwaysShow;
    [SerializeField] protected Color color = Color.white;
    [SerializeField] protected bool wire = true, fill = false;
    [SerializeField] protected Vector3 localCenter;

    private void OnDrawGizmos()
    {
        if (!alwaysShow)
            return;

        Gizmo();
    }

    private void OnDrawGizmosSelected()
    {
        if (alwaysShow)
            return;

        Gizmo();
    }

    public abstract void Wire();
    public abstract void Solid();

    public void Gizmo()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Color c = color;
        c.a = 1;
        Gizmos.color = c;
        if (wire)
            Wire();

        Gizmos.color = color;
        if (fill)
            Solid();
    }
}
