using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomAI.BehaviorTree
{
    public class ReactionNode : WaitNode
    {
        Actor actor;

        public ReactionNode(float delay, Actor actor) : base(delay)
        {
            this.actor = actor;
            timer = 0;
            this.delay = delay;
        }

        public override void OnStart()
        {
            base.OnStart();
            actor.ReactToTarget();
        }
    }
}