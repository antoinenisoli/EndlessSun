using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomAI.BehaviorTree
{
    public class AttackNode : AIActionNode
    {
        protected NPC myNPC;
        protected float speedMultiplier;

        protected float attackTimer;
        protected float stayDistantRange;
        protected float stepBackTimer, stepBackDelay;

        protected Vector2 oppositeDirection => (currentPosition - targetPos).normalized;
        protected Vector2 currentPosition => myNPC.transform.position;
        protected Vector2 targetPos => myNPC.MainTarget.transform.position;

        public AttackNode(NPC myNPC, float stayDistantRange, float stepBackDelay = 0.15f, float speedMultiplier = 1)
        {
            this.myNPC = myNPC;
            this.stayDistantRange = stayDistantRange;
            this.stepBackDelay = stepBackDelay;
            this.speedMultiplier = speedMultiplier;
        }

        public override bool Step()
        {
            if (attackTimer >= myNPC.attackRate)
                return true;

            StayAway();
            attackTimer += Time.deltaTime;
            return false;
        }

        void KeepDistance()
        {
            Vector2 awayVector = targetPos + oppositeDirection * stayDistantRange;
            myNPC.Move(awayVector, speedMultiplier);
        }

        protected virtual void MovingBehavior()
        {
            myNPC.Stop();
            stepBackTimer = 0;
        }

        void StayAway()
        {
            float distance = Vector2.Distance(currentPosition, targetPos);
            if (distance < stayDistantRange)
            {
                if (stepBackTimer > stepBackDelay)
                    KeepDistance();

                stepBackTimer += Time.deltaTime;
            }
            else
                MovingBehavior();             
        }

        public override void Execute()
        {
            attackTimer = 0;
            myNPC.Stop();
            myNPC.LaunchAttack();
        }
    }
}