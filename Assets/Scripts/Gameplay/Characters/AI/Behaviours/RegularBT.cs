using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomAI.BehaviorTree
{
    public class RegularBT : BehaviorTree
    {
        [SerializeField] PatrolData patrol;

        public override AINode MakeTree()
        {
            SequenceNode sequence = new SequenceNode();
            WaitNode wait = new WaitNode(patrol.randomDelayBounds);
            PatrolNode patrolNode = new PatrolNode(patrol);

            sequence.Attach(wait);
            sequence.Attach(patrolNode);
            return sequence;
        }
    }
}