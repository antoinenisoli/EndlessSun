using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomAI.BehaviorTree
{
    public class CheckHealthNode : AIConditionNode
    {
        Actor actor;
        float threshold;

        public CheckHealthNode(Actor actor, float threshold)
        {
            this.actor = actor;
            this.threshold = threshold;
        }

        public override bool Check()
        {
            float compute = actor.Health.CurrentValue / actor.Health.MaxValue;
            return compute <= (float)threshold/100;
        }
    }
}