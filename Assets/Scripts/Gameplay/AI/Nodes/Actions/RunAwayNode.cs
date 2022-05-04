using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomAI.BehaviorTree
{
    public class RunAwayNode : AIActionNode
    {
        Actor actor;
        float maxDistance = 2f;

        public RunAwayNode(Actor actor, float maxDistance)
        {
            this.actor = actor;
            this.maxDistance = maxDistance;
        }

        public Vector2 TargetPosition()
        {
            if (actor.Target)
                return actor.Target.transform.position;
            else
                return new Vector2();
        }

        public bool FarEnough()
        {
            float distance = Vector2.Distance(TargetPosition(), actor.transform.position);
            return distance > maxDistance;
        }

        public override bool Step()
        {
            Vector2 oppositeDirection = ((Vector2)actor.transform.position - TargetPosition()).normalized;
            actor.Move(TargetPosition() + oppositeDirection * maxDistance);
            return FarEnough();
        }

        public override void Execute()
        {
            actor.SetTarget(null);
            actor.Stop();
        }
    }
}
