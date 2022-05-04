using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomAI.BehaviorTree
{
    public class AttackInCircleNode : AttackNode
    {
        float timer, cadency;
        int direction = 1;
        bool isContinuous;
        Vector2 randomRange;

        public AttackInCircleNode(NPC myNPC, float stayDistantRange, bool isContinuous = true, Vector2 randomRange = default, float stepBackDelay = 0.15f) : base(myNPC, stayDistantRange, stepBackDelay)
        {
            this.isContinuous = isContinuous;
            this.randomRange = randomRange;
        }

        protected override void MovingBehavior()
        {
            if (isContinuous)
                direction = 1;
            else
            {
                timer += Time.deltaTime;
                if (timer > cadency)
                {
                    timer = 0;
                    direction = Random.Range(-1, 2);
                    cadency = GameDevHelper.RandomInRange(randomRange);
                }
            }

            myNPC.Move(CircleAround());
        }

        Vector2 CircleAround()
        {
            Vector2 perpendicular = Vector2.Perpendicular(oppositeDirection);
            return targetPos + (oppositeDirection + perpendicular * direction) * stayDistantRange;
        }
    }
}