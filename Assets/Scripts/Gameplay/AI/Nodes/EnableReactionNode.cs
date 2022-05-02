using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomAI.BehaviorTree
{
    public class EnableReactionNode : AINode
    {
        Actor actor;
        bool enable;

        public EnableReactionNode(Actor actor, bool enable)
        {
            this.actor = actor;
            this.enable = enable;
        }

        public override void OnStart()
        {
            base.OnStart();
            nodeState = NodeState.Running;
        }

        public override NodeState Evaluate()
        {
            actor.isReacting = enable;
            actor.Stop();
            if (enable)
                actor.ReactToTarget();

            nodeState = NodeState.Success;
            return nodeState;
        }
    }
}
