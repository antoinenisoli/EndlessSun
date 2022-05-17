using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomAI.BehaviorTree
{
    public class TrenchObserver : BehaviorTree
    {
        [Header(nameof(TrenchObserver))]
        public int healthThreshold = 0;
        [SerializeField] ShowSphereGizmo attackPointGizmo;

        [Space()]
        public PatrolData patrol;

        [Space()]
        public AttackData attack;

        [Header("Speeds")]
        [SerializeField] float chaseSpeed = 1.3f;
        [SerializeField] float fleeSpeed = 1.3f;

        [Header("Distance checks")]
        public DistanceCheck chaseRange = new DistanceCheck(2f, Color.white);
        public DistanceCheck aggroRange = new DistanceCheck(15f, Color.white);
        public DistanceCheck sightRange = new DistanceCheck(20f, Color.white);

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

        SequenceNode SetupFight()
        {
            SequenceNode fightSequence = new SequenceNode();
            CheckDistanceNode inSightNode = new CheckDistanceNode(myActor, sightRange.range, true);

            fightSequence.Attach(SetupGetTarget(), inSightNode, SetupChase());

            return fightSequence;
        }

        Selector SetupChase()
        {
            ChaseNode chaseNode = new ChaseNode(myActor, chaseRange.range, chaseSpeed);
            Selector chaseSelector = new Selector();

            chaseSelector.Attach(SetupAttack(), chaseNode);

            return chaseSelector;
        }

        SequenceNode SetupPatrol()
        {
            SequenceNode mainSequence = new SequenceNode();
            float randomDelay = GameDevHelper.RandomInRange(patrol.randomDelayBounds);
            WaitNode wait = new WaitNode(randomDelay);
            PatrolNode patrolNode = new PatrolNode(patrol);

            mainSequence.Attach(wait, patrolNode);

            return mainSequence;
        }

        SequenceNode SetupAttack()
        {
            SequenceNode mainSequence = new SequenceNode();
            CheckDistanceNode inRange = new CheckDistanceNode(myActor, attack.attackRange.range);
            AttackNode attackNode = attack.GenerateNode();

            mainSequence.Attach(SetupGetTarget(), inRange, attackNode);

            return mainSequence;
        }

        Selector SetupGetTarget()
        {
            Selector selector = new Selector();
            CheckTarget checkTarget = new CheckTarget(myActor);
            DetectTargetNode detectTarget = new DetectTargetNode(myActor, aggroRange.range);
            ReactionNode react = new ReactionNode(myActor);
            SequenceNode scanSequence = new SequenceNode();

            scanSequence.Attach(detectTarget, react);
            selector.Attach(checkTarget, scanSequence);
            return selector;
        }

        SequenceNode SetupHealthCheck()
        {
            SequenceNode mainSequence = new SequenceNode();
            CheckTarget checkTarget = new CheckTarget(myActor);
            CheckHealthNode checkHealth = new CheckHealthNode(myActor, healthThreshold);
            RunAwayNode runAway = new RunAwayNode(myActor, sightRange.range, fleeSpeed);

            mainSequence.Attach(checkTarget, checkHealth, runAway);

            return mainSequence;
        }

        public override AINode MakeTree()
        {
            Selector topSelector = new Selector();
            topSelector.Attach(SetupHealthCheck(), SetupFight(), SetupPatrol());
            return topSelector;
        }
    }
}