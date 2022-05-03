using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomAI.BehaviorTree
{
    public class IsReactingNode : AIConditionNode
    {
        Actor actor;

        public IsReactingNode(Actor actor)
        {
            this.actor = actor;
        }

        public override bool Check()
        {
            //Debug.Log(actor.isReacting);
            return actor.isReacting;
        }
    }
}
