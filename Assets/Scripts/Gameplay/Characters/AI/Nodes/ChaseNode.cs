using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseNode : AINode
{
    Transform target;
    NPC npc;

    public ChaseNode(Transform target, NPC npc)
    {
        this.target = target;
        this.npc = npc;
    }

    public override NodeState Evaluate()
    {
        float distance = Vector2.Distance(target.position, npc.transform.position);
        if (distance > 0.2f)
        {
            return NodeState.Running;
        }
        else
        {
            npc.Stop();
            return NodeState.Failure;
        }
    }
}
