using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomAI.BehaviorTree
{
    public class AttackNode : AIActionNode
    {
        NPC myNPC;
        Transform attackOrigin;
        float attackRange;

        float timer;
        Vector2 randomPosition;
        float randomTimer;
        float randomRate;

        Vector2 oppositeDirection => (currentPosition - targetPos).normalized;
        Vector2 currentPosition => myNPC.transform.position;
        Vector2 targetPos => myNPC.Target.transform.position;

        public AttackNode(NPC myNPC)
        {
            this.myNPC = myNPC;
        }

        public override bool Step()
        {
            if (timer >= myNPC.attackRate)
                return true;
            
            timer += Time.deltaTime;
            MoveAroundPlayer();
            return false;
        }

        public override void Execute()
        {
            timer = 0;
            myNPC.LaunchAttack();
        }

        void MoveAroundPlayer()
        {
            randomTimer += Time.deltaTime;
            if (randomTimer > randomRate)
                NewPos();

            myNPC.Move(randomPosition);
        }

        void NewPos()
        {
            randomRate = Random.Range(1f, 3f);
            randomTimer = 0;
            Vector2 perpendicular = Vector2.Perpendicular(oppositeDirection);
            int r = Mathf.RoundToInt(Random.Range(-1f, 1f));
            randomPosition = targetPos + oppositeDirection + perpendicular * r * 3f;
        }
    }
}