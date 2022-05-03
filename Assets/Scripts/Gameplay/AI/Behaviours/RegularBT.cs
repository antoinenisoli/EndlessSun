using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomAI.BehaviorTree
{
    public class RegularBT : BehaviorTree
    {
        [SerializeField] Transform attackPoint;

        [Space()]
        public DistanceCheck chaseRange = new DistanceCheck(2f, Color.white);
        public DistanceCheck aggroRange = new DistanceCheck(15f, Color.white);
        public DistanceCheck sightRange = new DistanceCheck(20f, Color.white);
        public DistanceCheck attackRange = new DistanceCheck(2f, Color.white);
        public PatrolData patrol;

        private void OnDrawGizmos()
        {
            patrol.Gizmos();
        }

        public Actor getActor() => GetComponent<Actor>();

        public override void Attack()
        {
            base.Attack();
            Collider2D[] colliders = Physics2D.OverlapCircleAll(attackPoint.transform.position, attackRange.range/2);
            foreach (var item in colliders)
            {
                Entity entity = item.GetComponentInParent<Entity>();
                if (entity && !entity.SameTeam(myActor))
                    entity.TakeDamages(myActor.ComputeDamages(), myActor);
            }
        }

        SequenceNode SetupChase()
        {
            SequenceNode mainSequence = new SequenceNode();
            Selector getTarget = new Selector();
            CheckTarget checkTarget = new CheckTarget(myActor);
            DetectTargetNode detectTarget = new DetectTargetNode(myActor, aggroRange.range);
            CheckDistanceNode inSightNode = new CheckDistanceNode(myActor, sightRange.range);
            ChaseNode chaseNode = new ChaseNode(myActor, chaseRange.range);
            EnableReactionNode react = new EnableReactionNode(myActor, true);
            SequenceNode scanSequence = new SequenceNode();

            scanSequence.Attach(detectTarget);
            scanSequence.Attach(react);

            getTarget.Attach(checkTarget);
            getTarget.Attach(scanSequence);

            mainSequence.Attach(getTarget);
            mainSequence.Attach(inSightNode);
            mainSequence.Attach(chaseNode);

            return mainSequence;
        }

        SequenceNode SetupPatrol()
        {
            SequenceNode mainSequence = new SequenceNode();
            float randomDelay = GameDevHelper.RandomInRange(patrol.randomDelayBounds);
            ResetActorNode resetActor = new ResetActorNode(myActor);
            WaitNode wait = new WaitNode(randomDelay);
            PatrolNode patrolNode = new PatrolNode(patrol);
            mainSequence.Attach(resetActor);
            mainSequence.Attach(wait);
            mainSequence.Attach(patrolNode);

            return mainSequence;
        }

        SequenceNode SetupAttack()
        {
            SequenceNode mainSequence = new SequenceNode();
            SequenceNode getTarget = new SequenceNode();
            CheckTarget checkTarget = new CheckTarget(myActor);
            CheckDistanceNode inRange = new CheckDistanceNode(myActor, attackRange.range);
            AttackNode attackNode = new AttackNode(myActor as NPC);

            getTarget.Attach(checkTarget);
            getTarget.Attach(inRange);

            mainSequence.Attach(getTarget);
            mainSequence.Attach(attackNode);

            return mainSequence;
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