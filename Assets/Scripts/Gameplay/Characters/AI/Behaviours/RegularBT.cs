using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomAI.BehaviorTree
{
    public class RegularBT : BehaviorTree
    {
        public DistanceCheck chaseRange = new DistanceCheck(2f, Color.white);
        public DistanceCheck aggroRange = new DistanceCheck(15f, Color.white);
        public DistanceCheck sightRange = new DistanceCheck(20f, Color.white);
        public DistanceCheck attackRange = new DistanceCheck(2f, Color.white);
        [SerializeField] float reactDuration = 0.5f;
        public PatrolData patrol;

        private void OnDrawGizmos()
        {
            patrol.Gizmos();
        }

        public override void ReactToTarget()
        {
            base.ReactToTarget();
            myActor.ReactToTarget();
        }

        public Actor getActor() => GetComponent<Actor>();

        SequenceNode SetupChase()
        {
            SequenceNode mainSequence = new SequenceNode();
            Selector getTarget = new Selector();
            CheckTarget checkTarget = new CheckTarget(myActor);
            DetectTargetNode detectTarget = new DetectTargetNode(myActor, aggroRange.range);
            CheckDistanceNode inSightNode = new CheckDistanceNode(myActor, sightRange.range);
            ChaseNode chaseNode = new ChaseNode(myActor, chaseRange.range);
            ReactionNode react = new ReactionNode(reactDuration, myActor);

            getTarget.Attach(checkTarget);
            getTarget.Attach(detectTarget);

            mainSequence.Attach(getTarget);
            mainSequence.Attach(react);
            mainSequence.Attach(inSightNode);
            mainSequence.Attach(chaseNode);

            return mainSequence;
        }

        SequenceNode SetupPatrol()
        {
            SequenceNode sequence = new SequenceNode();
            float randomDelay = GameDevHelper.RandomInRange(patrol.randomDelayBounds);
            WaitNode wait = new WaitNode(randomDelay);
            PatrolNode patrolNode = new PatrolNode(patrol);
            sequence.Attach(wait);
            sequence.Attach(patrolNode);

            return sequence;
        }

        SequenceNode SetupAttack()
        {
            SequenceNode sequence = new SequenceNode();
            LogNode log = new LogNode("attack !!!!");
            SequenceNode getTarget = new SequenceNode();
            CheckTarget checkTarget = new CheckTarget(myActor);
            CheckDistanceNode inRange = new CheckDistanceNode(myActor, attackRange.range);
            AttackNode attackNode = new AttackNode(myActor as NPC);

            getTarget.Attach(checkTarget);
            getTarget.Attach(inRange);

            sequence.Attach(getTarget);
            //sequence.Attach(attackNode);

            return sequence;
        }

        public override AINode MakeTree()
        {
            Selector topSelector = new Selector();
            topSelector.Attach(SetupAttack());
            topSelector.Attach(SetupChase());
            topSelector.Attach(SetupPatrol());
            return topSelector;
        }
    }
}