using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
class Attacking : SubBehavior
{
    public override AIState State => AIState.Attacking;
    float timer;

    public Attacking(AIStateMachineBehavior behavior) : base(behavior)
    {
        behavior.myNPC.Stop();
    }

    public override void Update()
    {
        base.Update();
        if (myNPC.Target.Health.isDead)
        {
            myNPC.SetTarget(null);
            behavior.SetBehaviour(new Wait(behavior, 2, AIState.Patrolling));
            return;
        }
        else if (!behavior.NearToTarget())
            behavior.SetBehaviour(new Chasing(behavior));

        if (timer >= myNPC.attackRate)
        {
            timer = 0;
            myNPC.LaunchAttack();
        }
        else
            timer += Time.deltaTime;
    }
}
