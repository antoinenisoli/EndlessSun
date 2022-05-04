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
        float behaviorSpeed;

        public AttackInCircleNode(NPC myNPC, float stayDistantRange, float stepBackDelay = 0.15f, float speedMultiplier = 1) : base(myNPC, stayDistantRange, stepBackDelay, speedMultiplier)
        {
            
        }

        public void SetMovingBehavior(bool isContinuous, float behaviorSpeed = 1f, Vector2 randomRange = default)
        {
            this.isContinuous = isContinuous;
            this.randomRange = randomRange;
            this.behaviorSpeed = behaviorSpeed;
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

            myNPC.Move(CircleAround(), behaviorSpeed);
        }

        Vector2 CircleAround()
        {
            Vector2 perpendicular = Vector2.Perpendicular(oppositeDirection);
            return targetPos + (oppositeDirection + perpendicular * direction) * stayDistantRange;
        }
    }
}