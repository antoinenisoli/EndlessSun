using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomAI.BehaviorTree
{
    public class WaitNode : AIActionNode
    {
        protected float timer;
        protected float delay;

        public WaitNode(float delay)
        {
            timer = 0;
            this.delay = delay;
        }

        public override void Execute()
        {
            //Debug.Log("wait end");
        }

        public override bool Step()
        {
            timer += Time.deltaTime;
            //Debug.Log(timer + " " + nodeState);
            return timer >= delay;
        }
    }
}