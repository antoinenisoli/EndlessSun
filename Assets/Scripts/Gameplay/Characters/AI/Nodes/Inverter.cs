using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inverter : AINode
{
    protected AINode node;

    public Inverter(AINode node)
    {
        this.node = node;
    }

    public override NodeState Evaluate()
    {
        switch (node.Evaluate())
        {
            case NodeState.Running:
                nodeState = NodeState.Running;
                break;
            case NodeState.Success:
                nodeState = NodeState.Failure;
                break;
            case NodeState.Failure:
                nodeState = NodeState.Success;
                break;
            default:
                break;
        }

        return nodeState;
    }
}
