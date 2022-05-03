using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomAI.BehaviorTree
{
    public class CheckDistanceNode : AIConditionNode
    {
        Actor actor;
        float range;
        bool eraseTarget;

        public CheckDistanceNode(Actor actor, float range, bool eraseTarget = false)
        {
            this.actor = actor;
            this.range = range;
            this.eraseTarget = eraseTarget;
        }

        public override bool Check()
        {
            if (actor.Target)
            {
                float distance = Vector2.Distance(actor.Target.transform.position, actor.transform.position);
                if (distance < range)
                    return true;
            }

            return false;
        }
    }
}