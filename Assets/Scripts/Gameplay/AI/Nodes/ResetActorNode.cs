using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomAI.BehaviorTree
{
    public class ResetActorNode : AIActionNode
    {
        Actor actor;
        bool done;

        public ResetActorNode(Actor actor)
        {
            this.actor = actor;
        }

        public override void Execute()
        {
            Debug.Log("reset");
            done = true;
            actor.Stop();
            actor.SetTarget(null);
        }

        public override bool Step()
        {
            return actor.Target || !done;
        }
    }
}
