using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
class Attacking : StateMachineBehavior
{
    public override AIState State => AIState.Attacking;
    float timer;

    public Attacking(RegularBehavior behavior) : base(behavior)
    {
        behavior.myEntity.Stop();
    }

    public override void Update()
    {
        base.Update();
        if (myEntity.Target.Health.isDead)
        {
            myEntity.SetTarget(null);
            behavior.SetBehaviour(new Wait(behavior, 2, AIState.Patrolling));
            return;
        }
        else if (!myEntity.NearToTarget())
            behavior.SetBehaviour(new Chasing(behavior));

        if (timer >= myEntity.attackRate)
        {
            timer = 0;
            myEntity.LaunchAttack();
        }
        else
            timer += Time.deltaTime;
    }
}
