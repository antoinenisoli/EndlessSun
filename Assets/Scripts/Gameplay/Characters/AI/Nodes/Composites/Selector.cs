using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomAI.BehaviorTree
{
    public class Selector : AICompositeNode
    {
        public override NodeState Evaluate()
        {
            foreach (var node in childrens)
            {
                //Debug.Log(node.name + " " + node.nodeState);
                switch (node.Evaluate())
                {
                    case NodeState.Running:
                        if (!node.GetType().IsSubclassOf(typeof(AICompositeNode)))
                            currentNode = node;

                        nodeState = NodeState.Running;
                        return nodeState;
                    case NodeState.Success:
                        nodeState = NodeState.Success;
                        return nodeState;
                }
            }

            nodeState = NodeState.Failure;
            return nodeState;
        }
    }
}