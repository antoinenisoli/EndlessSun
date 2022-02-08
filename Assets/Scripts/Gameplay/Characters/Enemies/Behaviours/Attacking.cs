using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
class Attacking : EnemyBehaviour
{
    public override EnemyState State => EnemyState.Attacking;
    float timer;

    public Attacking(Enemy myEnemy) : base(myEnemy)
    {
        myEnemy.Stop();
    }

    public override void Update()
    {
        base.Update();
        if (myEnemy.Target.Health.isDead)
        {
            myEnemy.SetTarget(null);
            myEnemy.SetBehaviour(new Wait(myEnemy, 2, EnemyState.Patrolling));
            return;
        }
        else if (!myEnemy.NearToTarget())
            myEnemy.SetBehaviour(new Chasing(myEnemy));

        if (timer >= myEnemy.attackRate)
        {
            timer = 0;
            myEnemy.LaunchAttack();
        }
        else
            timer += Time.deltaTime;
    }
}
