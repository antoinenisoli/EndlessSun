using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomAI.BehaviorTree
{
    public class TargetInSightNode : AIConditionNode
    {
        Actor actor;
        float sightRange;

        public TargetInSightNode(Actor actor, float sightRange)
        {
            this.actor = actor;
            this.sightRange = sightRange;
        }

        public override bool Check()
        {
            if (actor.Target)
            {
                float distance = Vector2.Distance(actor.Target.transform.position, actor.transform.position);
                if (distance < sightRange)
                    return true;
            }

            actor.SetTarget(null);
            return false;
        }
    }
}