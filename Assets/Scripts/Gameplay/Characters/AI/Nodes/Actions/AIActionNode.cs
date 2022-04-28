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
            if (Step())
                return NodeState.Success;

            Execute();
            return NodeState.Running;
        }
    }
}
