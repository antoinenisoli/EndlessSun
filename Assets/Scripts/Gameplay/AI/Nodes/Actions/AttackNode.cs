using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomAI.BehaviorTree
{
    public class AttackNode : AIActionNode
    {
        protected NPC myNPC;
        protected float attackTimer;

        protected float stayDistantRange;
        float stepBackTimer, stepBackDelay;
        bool canStepBack;

        protected Vector2 oppositeDirection => (currentPosition - targetPos).normalized;
        protected Vector2 currentPosition => myNPC.transform.position;
        protected Vector2 targetPos => myNPC.Target.transform.position;

        public AttackNode(NPC myNPC, float stayDistantRange)
        {
            this.myNPC = myNPC;
            this.stayDistantRange = stayDistantRange;
        }

        public override bool Step()
        {
            if (attackTimer >= myNPC.attackRate)
                return true;

            if (stepBackTimer > stepBackDelay && canStepBack)
            {
                stepBackTimer = 0;

            }

            StayAway();
            attackTimer += Time.deltaTime;
            return false;
        }

        protected virtual void StayAway()
        {
            float distance = Vector2.Distance(currentPosition, targetPos);
            if (distance < stayDistantRange)
            {
                stepBackTimer = 0;
                Vector2 awayVector = targetPos + oppositeDirection * stayDistantRange;
                myNPC.Move(awayVector);
            }
            else
            {
                myNPC.Stop();
                stepBackTimer += Time.deltaTime;
            }
        }

        public override void Execute()
        {
            attackTimer = 0;
            myNPC.Stop();
            myNPC.LaunchAttack();
        }
    }
}