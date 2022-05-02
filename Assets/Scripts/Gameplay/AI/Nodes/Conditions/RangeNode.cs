using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomAI.BehaviorTree
{
    public class RangeNode : AIConditionNode
    {
        float range;
        Transform target, origin;

        public RangeNode(float range, Transform target, Transform origin)
        {
            this.range = range;
            this.target = target;
            this.origin = origin;
        }

        public override bool Check()
        {
            float distance = Vector2.Distance(target.position, origin.position);
            return distance <= range;
        }
    }
}