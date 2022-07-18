using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomAI.BehaviorTree
{
    public class IsFarNode : CheckDistanceNode
    {
        public IsFarNode(Actor actor, float range, bool eraseTarget = false) : base(actor, range, eraseTarget)
        {
            this.actor = actor;
            this.range = range;
            this.eraseTarget = eraseTarget;
        }

        protected override bool DistanceType()
        {
            float distance = Vector2.Distance(actor.MainTarget.transform.position, actor.transform.position);
            return distance > range;
        }
    }
}
