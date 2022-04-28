using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomAI.BehaviorTree
{
    public class SequenceNode : AICompositeNode
    {
        public override NodeState Evaluate()
        {
            bool isAnyNodeRunning = false;
            foreach (var node in childrens)
            {
                Debug.Log(node.name + " / " + node.nodeState);
                switch (node.Evaluate())
                {
                    case NodeState.Running:
                        isAnyNodeRunning = true;
                        nodeState = NodeState.Running;
                        return nodeState;
                    case NodeState.Success:
                        continue;
                    case NodeState.Failure:
                        nodeState = NodeState.Failure;
                        return nodeState;
                }
            }

            nodeState = isAnyNodeRunning ? NodeState.Running : NodeState.Success;
            return nodeState;
        }
    }
}