using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomAI.BehaviorTree
{
    public class HealthNode : AIConditionNode
    {
        Actor actor;
        float threshold;

        public HealthNode(Actor actor, float threshold)
        {
            this.actor = actor;
            this.threshold = threshold;
        }

        public override bool Check()
        {
            return actor.Health.CurrentValue <= threshold;
        }
    }
}