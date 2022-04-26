using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Selector : AINode
{
    protected List<AINode> nodes = new List<AINode>();

    public Selector(List<AINode> nodes)
    {
        this.nodes = nodes;
    }

    public override NodeState Evaluate()
    {
        foreach (var node in nodes)
        {
            switch (node.Evaluate())
            {
                case NodeState.Running:
                    nodeState = NodeState.Running;
                    return nodeState;
                case NodeState.Success:
                    nodeState = NodeState.Success;
                    return nodeState;
                case NodeState.Failure:
                    break;
                default:
                    break;
            }
        }

        nodeState = NodeState.Failure;
        return nodeState;
    }
}
