using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomAI
{
    [System.Serializable]
    class Attacking : SubBehavior
    {
        public override AIState State => AIState.Attacking;
        float timer;
        Vector2 randomPosition;
        float randomTimer;
        float randomRate;

        Vector2 oppositeDirection => (currentPosition - targetPos).normalized;
        Vector2 currentPosition => myNPC.transform.position;
        Vector2 targetPos => myNPC.Target.transform.position;

        public Attacking(AIStateMachineBehavior behavior) : base(behavior)
        {
            myNPC.Stop();
            randomTimer = 0;
            randomPosition = targetPos;
        }

        public override void Update()
        {
            base.Update();
            Main();
            MoveAroundPlayer();
        }

        void Main()
        {
            if (myNPC.Target.Health.isDead)
            {
                myNPC.SetTarget(null);
                behavior.SetBehaviour(new Wait(behavior, 2, AIState.Patrolling));
                return;
            }

            if (timer >= myNPC.attackRate)
            {
                timer = 0;
                myNPC.LaunchAttack();
            }
            else
                timer += Time.deltaTime;
        }

        void MoveAroundPlayer()
        {
            randomTimer += Time.deltaTime;
            if (randomTimer > randomRate)
                NewPos();

            behavior.Move(randomPosition);
        }

        void NewPos()
        {
            randomRate = Random.Range(1f, 3f);
            randomTimer = 0;
            Vector2 perpendicular = Vector2.Perpendicular(oppositeDirection);
            int r = Mathf.RoundToInt(Random.Range(-1f, 1f));
            randomPosition = targetPos + oppositeDirection * behavior.patrol.chaseMinDistance + perpendicular * r * 3f;
        }
    }
}