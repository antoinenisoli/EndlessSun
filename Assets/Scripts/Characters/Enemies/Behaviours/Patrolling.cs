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

    public Patrolling(Enemy myEnemy) : base(myEnemy)
    {
        newPatrolDelay = myEnemy.RandomDelay();
        pos = myEnemy.RandomPatrolPosition();
    }

    void NewDestination()
    {
        myEnemy.Stop();
        newPatrolTimer = 0;
        newPatrolDelay = myEnemy.RandomDelay();
        Vector2 randomPos = myEnemy.RandomPatrolPosition();
        Vector2 sampledPos;
        if (GridManager.Instance)
        {
            randomPos = myEnemy.RandomPatrolPosition();
            sampledPos = GridManager.Instance.ClosestWalkable(randomPos);
            pos = sampledPos;
        }
        else
            pos = randomPos;
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
                NewDestination();

            myEnemy.Move(pos);
        }
    }
}
