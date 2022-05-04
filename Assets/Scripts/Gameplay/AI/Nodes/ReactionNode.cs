using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomAI.BehaviorTree
{
    public class ReactionNode : AIActionNode
    {
        Actor actor;

        public ReactionNode(Actor actor)
        {
            this.actor = actor;
        }

        public override void Execute()
        {
            //Debug.Log("reaction");
            actor.ReactToTarget();
        }

        public override bool Step()
        {
            return true;
        }
    }
}
