using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomAI.BehaviorTree
{
    public class ResetActorNode : AINode
    {
        Actor actor;

        public ResetActorNode(Actor actor)
        {
            this.actor = actor;
        }

        public override NodeState Evaluate()
        {
            actor.SetTarget(null);
            actor.Stop();
            nodeState = NodeState.Success;
            return nodeState;
        }
    }
}
