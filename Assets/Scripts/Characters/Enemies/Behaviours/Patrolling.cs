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
    Vector3 pos;

    public Patrolling(Entity target, Enemy myEnemy) : base(target, myEnemy)
    {
        delay = myEnemy.RandomDelay();
        pos = myEnemy.RandomPatrolPosition();
    }

    public override void Update()
    {
        base.Update();
        if (myEnemy.DetectTargets(target.transform.position))
            myEnemy.SetBehaviour(new Chasing(target, myEnemy));
        else
        {
            patrolTimer += Time.deltaTime;
            if (patrolTimer > delay && !myEnemy.isMoving())
            {
                patrolTimer = 0;
                delay = myEnemy.RandomDelay();
                pos = myEnemy.RandomPatrolPosition();
            }

            myEnemy.Move(pos);
        }
    }
}
