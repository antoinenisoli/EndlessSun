using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomAI.BehaviorTree
{
    public class AttackInCircleNode : AttackNode
    {
        public AttackInCircleNode(NPC myNPC, float stayDistantRange) : base(myNPC, stayDistantRange)
        {
            
        }

        protected override void StayAway()
        {
            myNPC.Move(CircleAround());
        }

        Vector2 CircleAround()
        {
            Vector2 perpendicular = Vector2.Perpendicular(oppositeDirection);
            return targetPos + (oppositeDirection + perpendicular) * stayDistantRange;
        }
    }
}