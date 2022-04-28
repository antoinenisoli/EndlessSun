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
            foreach (var node in childrens)
            {
                //Debug.Log(node.name + " " + node.nodeState);
                switch (node.Evaluate())
                {
                    case NodeState.Running:
                        nodeState = NodeState.Running;
                        return nodeState;
                    case NodeState.Failure:
                        nodeState = NodeState.Failure;
                        return nodeState;
                }
            }

            nodeState = NodeState.Success;
            return nodeState;
        }
    }
}