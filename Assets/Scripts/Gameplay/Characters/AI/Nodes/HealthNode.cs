using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthNode : AINode
{
    Actor actor;
    float threshold;

    public HealthNode(Actor actor, float threshold)
    {
        this.actor = actor;
        this.threshold = threshold;
    }

    public override NodeState Evaluate()
    {
        return actor.Health.CurrentValue <= threshold ? NodeState.Success : NodeState.Failure;
    }
}
