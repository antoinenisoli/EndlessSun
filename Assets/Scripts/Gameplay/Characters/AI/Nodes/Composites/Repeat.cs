using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomAI.BehaviorTree
{
    public class Repeat : AICompositeNode
    {
        AICompositeNode composite;

        public Repeat(AICompositeNode composite)
        {
            this.composite = composite;
        }

        public override NodeState Evaluate()
        {
            if (composite.Evaluate() != NodeState.Running)
            {

            }

            return NodeState.Failure;
        }
    }
}