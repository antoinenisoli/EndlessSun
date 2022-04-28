using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomAI.BehaviorTree
{
    public class AttackNode : AINode
    {
        NPC myNpc;

        public AttackNode(NPC myNpc)
        {
            this.myNpc = myNpc;
        }

        public override NodeState Evaluate()
        {
            myNpc.Stop();
            return NodeState.Running;
        }
    }
}