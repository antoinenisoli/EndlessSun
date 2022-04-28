using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomAI.BehaviorTree
{
    public abstract class AIActionNode : AINode
    {
        public abstract void Execute();
        public abstract bool Step();

        public override NodeState Evaluate()
        {
            StartCheck();
            if (nodeState == NodeState.Success)
                return nodeState;

            if (Step())
            {
                nodeState = NodeState.Success;
                Execute();
                StopCheck();
                return nodeState;
            }

            StopCheck();
            nodeState = NodeState.Running;
            return nodeState;
        }
    }
}
