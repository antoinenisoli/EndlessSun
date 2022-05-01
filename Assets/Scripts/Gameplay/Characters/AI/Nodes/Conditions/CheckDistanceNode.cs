using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomAI.BehaviorTree
{
    public class CheckDistanceNode : AIConditionNode
    {
        Actor actor;
        float range;

        public CheckDistanceNode(Actor actor, float range)
        {
            this.actor = actor;
            this.range = range;
        }

        public override bool Check()
        {
            if (actor.Target)
            {
                float distance = Vector2.Distance(actor.Target.transform.position, actor.transform.position);
                Debug.Log(distance);
                if (distance < range)
                    return true;
            }

            actor.SetTarget(null);
            return false;
        }
    }
}