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
            if (GridManager.Instance.SamplePosition(randomPos, 2f, out sampledPos))
                pos = sampledPos;
        }
        else
            pos = randomPos;
    }

    public override void Gizmos()
    {
        base.Gizmos();
        myEnemy.patrolGizmo.gameObject.SetActive(true);
        myEnemy.aggroGizmo.gameObject.SetActive(true);

        if (myEnemy.patrolGizmo)
            myEnemy.patrolGizmo.SetSize(myEnemy.randomPatrolRange * 2);
        if (myEnemy.aggroGizmo)
            myEnemy.aggroGizmo.SetSize(myEnemy.aggroDistance);
    }

    public override void Update()
    {
        base.Update();
        if (myEnemy.DetectTarget(out Entity target))
        {
            myEnemy.NewAgressor(target);
        }
        else if (!myEnemy.isMoving())
        {
            newPatrolTimer += Time.deltaTime;
            if (newPatrolTimer > newPatrolDelay)
                NewDestination();

            myEnemy.Move(pos);
        }
    }
}
