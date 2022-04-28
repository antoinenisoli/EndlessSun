using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomAI.BehaviorTree
{
    public class ChaseNode : AINode
    {
        Actor actor;
        float chaseMinDistance = 2f;

        public ChaseNode(Actor actor, float chaseMinDistance)
        {
            this.actor = actor;
            this.chaseMinDistance = chaseMinDistance;
        }

        public bool NearToTarget()
        {
            if (!actor.Target)
                return false;

            float distance = Vector2.Distance(actor.Target.transform.position, actor.transform.position);
            return distance < chaseMinDistance;
        }

        public override NodeState Evaluate()
        {
            if (!NearToTarget())
            {
                actor.Move(actor.Target.transform.position);
                nodeState = NodeState.Running;
                return nodeState;
            }
            else
            {
                actor.Stop();
                nodeState = NodeState.Success;
                return nodeState;
            }
        }
    }
}