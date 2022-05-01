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
            this.delay = delay;
            timer = 0;
        }

        public override void OnStart()
        {
            base.OnStart();
            timer = 0;
            actor.ReactToTarget();
        }
    }
}