using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SequenceNode : AINode
{
    protected List<AINode> nodes = new List<AINode>();

    public SequenceNode(List<AINode> nodes)
    {
        this.nodes = nodes;
    }

    public override NodeState Evaluate()
    {
        bool isAnyNodeRunning = false;
        foreach (var node in nodes)
        {
            switch (node.Evaluate())
            {
                case NodeState.Running:
                    isAnyNodeRunning = true;
                    break;
                case NodeState.Success:
                    break;
                case NodeState.Failure:
                    nodeState = NodeState.Failure;
                    return nodeState;
                default:
                    break;
            }
        }

        nodeState = isAnyNodeRunning ? NodeState.Running : NodeState.Success;
        return nodeState;
    }
}
