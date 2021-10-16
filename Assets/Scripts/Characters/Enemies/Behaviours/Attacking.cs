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

    public Attacking(CharacterController player, Enemy myEnemy) : base(player, myEnemy)
    {
        
    }

    public override void Update()
    {
        base.Update();
        /*myEnemy.Stop();
        if (timer >= myEnemy.attackRate)
        {
            timer = 0;
            myEnemy.Attack(player);
        }
        else
            timer += Time.deltaTime;

        if (!myEnemy.NearPlayer(true))
            myEnemy.SetBehaviour(new Chasing(player, myEnemy));
        else if (!myEnemy.NearPlayer(false))
            myEnemy.SetBehaviour(new Patrolling(player, myEnemy));*/
    }
}
