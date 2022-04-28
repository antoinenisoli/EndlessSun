using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomAI
{
    [Serializable]
    public class Patrolling : SubBehavior
    {
        public override AIState State => AIState.Patrolling;
        PatrolData patrol => behavior.patrol;

        float newPatrolTimer;
        float newPatrolDelay;
        Vector3 pos;

        public Patrolling(AIStateMachineBehavior behavior) : base(behavior)
        {
            newPatrolDelay = patrol.RandomDelay();
            pos = patrol.NewDestination();
            myNPC.aggressors.Clear();
        }

        public void Move(Vector3 targetPos)
        {
            float distance = Vector2.Distance(targetPos, myNPC.transform.position);
            if (distance > myNPC.aiAgent.endReachedDistance)
                myNPC.Move(targetPos);
            else
                myNPC.Stop();
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
                {
                    newPatrolTimer = 0;
                    newPatrolDelay = patrol.RandomDelay();
                    pos = patrol.NewDestination();
                }

                Move(pos);
            }
        }
    }
}