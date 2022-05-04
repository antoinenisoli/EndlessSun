using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomAI.BehaviorTree
{
    public class ChaseNode : AIActionNode
    {
        Actor actor;
        float chaseMinDistance = 2f;
        float speedMultiplier;

        public ChaseNode(Actor actor, float chaseMinDistance, float speedMultiplier = 1)
        {
            this.actor = actor;
            this.chaseMinDistance = chaseMinDistance;
            this.speedMultiplier = speedMultiplier;
        }

        public bool NearToTarget()
        {
            float distance = Vector2.Distance(actor.MainTarget.transform.position, actor.transform.position);
            return distance < chaseMinDistance;
        }

        public override bool Step()
        {
            actor.Move(actor.MainTarget.transform.position, speedMultiplier);
            return NearToTarget();
        }

        public override void Execute()
        {
            actor.Stop();
        }
    }
}