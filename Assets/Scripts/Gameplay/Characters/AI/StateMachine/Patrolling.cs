using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Patrolling : StateMachineBehavior
{
    public override AIState State => AIState.Patrolling;
    float newPatrolTimer;
    float newPatrolDelay;
    Vector3 pos;

    public Patrolling(RegularBehavior behavior) : base(behavior)
    {
        newPatrolDelay = myEnemy.RandomDelay();
        pos = myEnemy.RandomPatrolPosition();
    }

    void NewDestination()
    {
        myEntity.Stop();
        newPatrolTimer = 0;
        newPatrolDelay = myEntity.RandomDelay();
        Vector2 randomPos = myEntity.RandomPatrolPosition();
        Vector2 sampledPos;
        if (GridManager.Instance)
        {
            randomPos = myEntity.RandomPatrolPosition();
            if (GridManager.Instance.SamplePosition(randomPos, 2f, out sampledPos))
                pos = sampledPos;
        }
        else
            pos = randomPos;
    }

    public override void Gizmos()
    {
        base.Gizmos();
        behavior.patrolGizmo.gameObject.SetActive(true);
        behavior.aggroGizmo.gameObject.SetActive(true);

        if (behavior.patrolGizmo)
            behavior.patrolGizmo.SetSize(behavior.randomPatrolRange * 2);
        if (behavior.aggroGizmo)
            behavior.aggroGizmo.SetSize(behavior.aggroDistance);
    }

    public override void Update()
    {
        base.Update();
        if (myEntity.DetectTarget(out Entity target))
        {
            myEntity.NewAgressor(target);
        }
        else if (!myEntity.isMoving())
        {
            newPatrolTimer += Time.deltaTime;
            if (newPatrolTimer > newPatrolDelay)
                NewDestination();

            myEntity.Move(pos);
        }
    }
}
