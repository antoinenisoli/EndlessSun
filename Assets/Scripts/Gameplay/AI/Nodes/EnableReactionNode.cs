using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomAI.BehaviorTree
{
    public class EnableReactionNode : AIActionNode
    {
        Actor actor;
        bool enable;

        public EnableReactionNode(Actor actor, bool enable)
        {
            this.actor = actor;
            this.enable = enable;
        }

        public override void Execute()
        {
            actor.isReacting = enable;
            //Debug.Log(enable);
            actor.Stop();
            if (enable)
                actor.ReactToTarget();
        }

        public override bool Step()
        {
            return true;
        }
    }
}
