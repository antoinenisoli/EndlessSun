using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomAI.BehaviorTree
{
    public class AttackNode : AIActionNode
    {
        NPC myNpc;

        public AttackNode(NPC myNpc)
        {
            this.myNpc = myNpc;
        }

        public override bool Step()
        {
            myNpc.Stop();
            return true;
        }

        public override void Execute()
        {
            
        }
    }
}