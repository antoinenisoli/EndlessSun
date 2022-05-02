using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomAI.BehaviorTree
{
    public class CheckTarget : AIConditionNode
    {
        Actor myActor;

        public CheckTarget(Actor myActor)
        {
            this.myActor = myActor;
        }

        public override bool Check()
        {
            if (!myActor.Target)
            {
                myActor.SetTarget(null);
                return false;
            }

            return true;
        }
    }
}
