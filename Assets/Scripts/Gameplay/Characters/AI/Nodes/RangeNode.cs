using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeNode : AINode
{
    float range;
    Transform target, origin;

    public RangeNode(float range, Transform target, Transform origin)
    {
        this.range = range;
        this.target = target;
        this.origin = origin;
    }

    public override NodeState Evaluate()
    {
        float distance = Vector2.Distance(target.position, origin.position);
        return distance <= range ? NodeState.Success : NodeState.Failure;
    }
}
