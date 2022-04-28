using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomAI.BehaviorTree
{
    public class ChaseNode : AINode
    {
        Transform target;
        NPC npc;
        public float patrolStopDistance = 0.1f, chaseMinDistance = 2f, attackMinDistance = 2f;

        public ChaseNode(Transform target, NPC npc)
        {
            this.target = target;
            this.npc = npc;
        }

        public bool NearToTarget()
        {
            if (!npc.Target)
                return false;

            float distance = Vector2.Distance(npc.Target.transform.position, npc.transform.position);
            return distance < chaseMinDistance;
        }

        public override NodeState Evaluate()
        {
            if (!NearToTarget())
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
}