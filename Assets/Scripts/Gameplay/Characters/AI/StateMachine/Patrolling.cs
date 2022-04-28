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

        public override void Update()
        {
            base.Update();
            if (patrol.DetectTarget(out Entity target))
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

                patrol.Move(pos);
            }
        }
    }
}