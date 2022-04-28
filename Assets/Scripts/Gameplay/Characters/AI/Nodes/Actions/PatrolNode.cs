using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomAI.BehaviorTree
{
    public class PatrolNode : AINode
    {
        PatrolData patrol;
        Vector3 pos;

        public PatrolNode(PatrolData patrol) 
        {
            this.patrol = patrol;
            pos = patrol.NewDestination();
        }

        public override NodeState Evaluate()
        {
            float distance = Vector2.Distance(patrol.myNPC.transform.position, pos);
            if (distance <= patrol.stopDistance)
            {
                patrol.myNPC.Stop();
                nodeState = NodeState.Success;
                return nodeState;
            }

            patrol.Move(pos);
            nodeState = NodeState.Running;
            return nodeState;
        }
    }
}