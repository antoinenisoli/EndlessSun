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
            if (!myActor.MainTarget)
                return false;

            if (myActor.MainTarget.Health.isDead)
                return false;

            return true;
        }
    }
}
