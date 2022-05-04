using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomAI.BehaviorTree
{
    public class PatrolNode : AIActionNode
    {
        PatrolData patrol;
        Vector3 pos;

        public PatrolNode(PatrolData patrol) 
        {
            this.patrol = patrol;
            pos = patrol.NewDestination();
        }

        public override void Execute()
        {
            patrol.myNPC.Stop();
        }

        public override bool Step()
        {
            float distance = Vector2.Distance(patrol.myNPC.transform.position, pos);
            patrol.myNPC.Move(pos);
            return distance <= patrol.myNPC.aiAgent.endReachedDistance;
        }
    }
}