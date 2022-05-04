using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomAI.BehaviorTree
{
    public class RunAwayNode : AIActionNode
    {
        Actor actor;
        float maxDistance = 2f;
        float speedMultiplier;

        public RunAwayNode(Actor actor, float maxDistance, float speedMultiplier = 1)
        {
            this.actor = actor;
            this.maxDistance = maxDistance;
            this.speedMultiplier = speedMultiplier;
        }

        public Vector2 TargetPosition()
        {
            return actor.MainTarget.transform.position;
        }

        public bool FarEnough()
        {
            float distance = Vector2.Distance(TargetPosition(), actor.transform.position);
            return distance > maxDistance;
        }

        public override bool Step()
        {
            Vector2 oppositeDirection = ((Vector2)actor.transform.position - TargetPosition()).normalized;
            Vector2 fleePosition = TargetPosition() + oppositeDirection * maxDistance;
            actor.Move(fleePosition, speedMultiplier);
            return FarEnough();
        }

        public override void Execute()
        {
            actor.SetTarget(null);
            actor.Stop();
        }
    }
}
