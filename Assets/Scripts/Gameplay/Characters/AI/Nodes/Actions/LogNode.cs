using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomAI.BehaviorTree
{
    public class LogNode : AINode
    {
        string text;

        public LogNode(string text)
        {
            this.text = text;
        }

        public override NodeState Evaluate()
        {
            Debug.Log(text);
            nodeState = NodeState.Success;
            return nodeState;
        }
    }
}