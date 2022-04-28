using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomAI.BehaviorTree
{
    public class WaitNode : AINode
    {
        float timer;
        float randomDelay;

        public WaitNode(Vector2 range)
        {
            timer = 0;
            randomDelay = GameDevHelper.RandomInRange(range);
        }

        public override NodeState Evaluate()
        {
            timer += Time.deltaTime;
            //Debug.Log(timer);

            if (timer >= randomDelay)
            {
                Debug.Log("wait end");
                nodeState = NodeState.Success;
                return nodeState;
            }

            nodeState = NodeState.Running;
            return nodeState;
        }
    }
}