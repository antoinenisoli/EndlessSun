using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomAI.BehaviorTree
{
    public class TrenchObserver : BehaviorTree
    {
        [Header(nameof(TrenchObserver))]
        public int targetHalthThreshold = 0;
        [SerializeField] ShowSphereGizmo attackPointGizmo;

        [Space()]
        public PatrolData patrol;

        [Space()]
        public AttackData attack;

        [Header("Speeds")]
        [SerializeField] float chaseSpeed = 1.3f;
        [SerializeField] float fleeSpeed = 1.3f;

        [Header("Distance checks")]
        public DistanceCheck observationRange = new DistanceCheck(2f, Color.white);
        public DistanceCheck sightRange = new DistanceCheck(2f, Color.white);
        public DistanceCheck chaseRange = new DistanceCheck(2f, Color.white);

        private void OnDrawGizmos()
        {
            patrol.Gizmos();
            if (attackPointGizmo)
                attackPointGizmo.radius = attack.attackRadius;
        }

        public Actor getActor() => GetComponent<Actor>();

        public override void Attack()
        {
            base.Attack();
            attack.Attack();
        }

        SequenceNode SetupFlee()
        {
            SequenceNode sequence = new SequenceNode();
            IsCloseNode closeNode = new IsCloseNode(myActor, observationRange.range);
            RunAwayNode runAway = new RunAwayNode(myActor, sightRange.range, fleeSpeed);

            sequence.Attach(closeNode, runAway);
            return sequence;
        }

        Selector SetupGetTarget()
        {
            Selector selector = new Selector();
            CheckTarget checkTarget = new CheckTarget(myActor);
            CheckHealthNode checkHealth = new CheckHealthNode(myActor, targetHalthThreshold);
            ChaseNode chaseNode = new ChaseNode(myActor, chaseRange.range);

            selector.Attach(checkTarget, checkHealth, chaseNode);
            return selector;
        }

        public override AINode MakeTree()
        {
            Selector topSelector = new Selector();
            SequenceNode fleeSequence = SetupFlee();
            Selector getTargetSelector = SetupGetTarget();

            topSelector.Attach(fleeSequence, getTargetSelector);
            return topSelector;
        }
    }
}