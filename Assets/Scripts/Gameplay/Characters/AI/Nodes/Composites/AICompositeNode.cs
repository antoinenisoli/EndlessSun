using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomAI.BehaviorTree
{
    public abstract class AICompositeNode : AINode
    {
        public AINode currentNode;
    }
}