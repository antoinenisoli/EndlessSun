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

    public Attacking(Entity target, Enemy myEnemy) : base(target, myEnemy)
    {
        
    }

    public override void Update()
    {
        base.Update();
        if (timer >= myEnemy.attackRate)
        {
            timer = 0;
            myEnemy.Attack(target);
        }
        else
            timer += Time.deltaTime;

        if (!myEnemy.NearToTarget(target.transform.position))
            myEnemy.SetBehaviour(new Chasing(target, myEnemy));
        else if (!myEnemy.DetectTargets(target.transform.position))
            myEnemy.SetBehaviour(new Patrolling(target, myEnemy));
    }
}
