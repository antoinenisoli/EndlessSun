using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Patrolling : EnemyBehaviour
{
    public override EnemyState State => EnemyState.Patrolling;
    float newPatrolTimer;
    float newPatrolDelay;
    Vector3 pos;
    float cooldownTimer;

    public Patrolling(Enemy myEnemy) : base(myEnemy)
    {
        newPatrolDelay = myEnemy.RandomDelay();
        pos = myEnemy.RandomPatrolPosition();
    }

    public override void Update()
    {
        base.Update();
        if (myEnemy.DetectTargets())
            myEnemy.SetBehaviour(new Reacting(myEnemy, myEnemy.reactTimer));
        else if (!myEnemy.isMoving())
        {
            newPatrolTimer += Time.deltaTime;
            if (newPatrolTimer > newPatrolDelay)
            {
                myEnemy.Stop();
                newPatrolTimer = 0;
                newPatrolDelay = myEnemy.RandomDelay();
                pos = myEnemy.RandomPatrolPosition();
            }

            myEnemy.Move(pos);
        }
    }
}
