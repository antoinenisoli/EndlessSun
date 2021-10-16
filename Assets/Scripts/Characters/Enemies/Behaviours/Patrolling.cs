using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Patrolling : EnemyBehaviour
{
    public override EnemyState State => EnemyState.Patrolling;
    float patrolTimer;
    float delay;

    public Patrolling(CharacterController player, Enemy myEnemy) : base(player, myEnemy)
    {
        //delay = myEnemy.RandomDelay();
    }

    public override void Update()
    {
        base.Update();
        /*if (myEnemy.NearPlayer(false))
            myEnemy.SetBehaviour(new Chasing(player, myEnemy));
        else
        {
            patrolTimer += Time.deltaTime;
            if (patrolTimer > delay)
            {
                patrolTimer = 0;
                delay = myEnemy.RandomDelay();
                myEnemy.PatrolMove();
            }
        }*/
    }
}
