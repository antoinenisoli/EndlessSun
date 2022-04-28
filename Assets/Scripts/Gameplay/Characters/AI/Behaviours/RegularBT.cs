using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomAI.BehaviorTree
{
    public class RegularBT : BehaviorTree
    {
        [SerializeField] PatrolData patrol;

        private void OnDrawGizmos()
        {
            patrol.Gizmos();
        }

        public override AINode MakeTree()
        {
            SequenceNode sequence = new SequenceNode();
            WaitNode wait = new WaitNode(patrol.randomDelayBounds);
            PatrolNode patrolNode = new PatrolNode(patrol);

            sequence.Attach(patrolNode);
            sequence.Attach(wait);

            return sequence;
        }
    }
}