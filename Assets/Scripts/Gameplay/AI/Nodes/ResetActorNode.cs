using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomAI.BehaviorTree
{
    public class ResetActorNode : AIActionNode
    {
        Actor actor;

        public ResetActorNode(Actor actor)
        {
            this.actor = actor;
        }

        public override void Execute()
        {
            Debug.Log("reset");
            actor.SetTarget(null);
        }

        public override bool Step()
        {
            return actor.Target;
        }
    }
}
