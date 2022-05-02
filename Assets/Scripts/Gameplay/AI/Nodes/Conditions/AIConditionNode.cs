using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomAI.BehaviorTree
{
    public abstract class AIConditionNode : AINode
    {
        public abstract bool Check();

        public override NodeState Evaluate()
        {
            StartCheck();
            if (Check())
            {
                nodeState = NodeState.Success;
                return nodeState;
            }
            else
            {
                nodeState = NodeState.Failure;
                return nodeState;
            }
        }
    }
}
