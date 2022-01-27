using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SphereCastVisual
{
    [SerializeField] bool debugSpherecast = true;
    [SerializeField] [Range(1, 10)] int debugIterations = 1;
    public float castRadius = 0.5f;
    public Gradient colors;

    public void VisualizeCast(LayerMask groundLayer, Ray ray, float range = 3f)
    {
        if (!debugSpherecast)
            return;

        bool detectObstacle = Physics.SphereCast(ray.origin, castRadius, ray.direction, out RaycastHit hit, range, groundLayer);
        Gizmos.color = colors.Evaluate(Convert.ToInt32(detectObstacle));

        if (hit.transform)
        {
            float dist = Vector3.Distance(ray.origin, hit.point);
            Gizmos.DrawRay(ray.origin, ray.direction * dist);

            for (int i = debugIterations; i < dist; i += debugIterations)
                Gizmos.DrawWireSphere(ray.GetPoint(i), castRadius);
        }
        else
        {
            Gizmos.DrawRay(ray.origin, ray.direction * range);
            for (int i = debugIterations; i < range; i += debugIterations)
                Gizmos.DrawWireSphere(ray.GetPoint(i), castRadius);
        }
    }
}
