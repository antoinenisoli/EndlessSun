using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Patrolling : SubBehavior
{
    public override AIState State => AIState.Patrolling;
    float newPatrolTimer;
    float newPatrolDelay;
    Vector3 pos;

    public Patrolling(AIStateMachineBehavior behavior) : base(behavior)
    {
        newPatrolDelay = behavior.RandomDelay();
        pos = behavior.RandomPatrolPosition();
        myNPC.aggressors.Clear();
    }

    void NewDestination()
    {
        myNPC.Stop();
        newPatrolTimer = 0;
        newPatrolDelay = behavior.RandomDelay();
        Vector2 randomPos = behavior.RandomPatrolPosition();
        Vector2 sampledPos;
        if (GridManager.Instance)
        {
            randomPos = behavior.RandomPatrolPosition();
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
        if (behavior.DetectTarget(out Entity target))
        {
            myNPC.NewAgressor(target);
        }
        else if (!myNPC.IsMoving())
        {
            newPatrolTimer += Time.deltaTime;
            if (newPatrolTimer > newPatrolDelay)
                NewDestination();

            behavior.Move(pos);
        }
    }
}
