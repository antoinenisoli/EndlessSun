using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomAI.BehaviorTree
{
    public class Inverter : AICompositeNode
    {
        protected AINode node;

        public Inverter(AINode node)
        {
            this.node = node;
        }

        public override NodeState Evaluate()
        {
            switch (node.Evaluate())
            {
                case NodeState.Running:
                    nodeState = NodeState.Running;
                    break;
                case NodeState.Success:
                    nodeState = NodeState.Failure;
                    break;
                case NodeState.Failure:
                    nodeState = NodeState.Success;
                    break;
            }

            return nodeState;
        }
    }
}