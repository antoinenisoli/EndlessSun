using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomAI.BehaviorTree
{
    public enum NPCPersonnality
    {
        Normal,
        Coward,
    }

    public class RegularBT : BehaviorTree
    {
        [Header(nameof(RegularBT))]
        [SerializeField] NPCPersonnality personnality;
        public int healthThreshold = 0;
        [SerializeField] Transform attackPoint;

        [Space()]
        public DistanceCheck chaseRange = new DistanceCheck(2f, Color.white);
        public DistanceCheck aggroRange = new DistanceCheck(15f, Color.white);
        public DistanceCheck sightRange = new DistanceCheck(20f, Color.white);
        public PatrolData patrol;
        public AttackData attack;

        private void OnDrawGizmos()
        {
            patrol.Gizmos();
        }

        public Actor getActor() => GetComponent<Actor>();

        public override void Attack()
        {
            base.Attack();
            Collider2D[] colliders = Physics2D.OverlapCircleAll(attackPoint.transform.position, attack.attackRange.range/2);
            foreach (var item in colliders)
            {
                Entity entity = item.GetComponentInParent<Entity>();
                if (entity && !entity.SameTeam(myActor))
                    entity.TakeDamages(myActor.ComputeDamages(), myActor);
            }
        }

        SequenceNode SetupFight()
        {
            SequenceNode fightSequence = new SequenceNode();
            CheckDistanceNode inSightNode = new CheckDistanceNode(myActor, sightRange.range, true);

            fightSequence.Attach(SetupGetTarget());
            fightSequence.Attach(inSightNode);
            fightSequence.Attach(SetupChase());

            return fightSequence;
        }

        Selector SetupChase()
        {
            ChaseNode chaseNode = new ChaseNode(myActor, chaseRange.range);
            Selector chaseSelector = new Selector();
            chaseSelector.Attach(SetupAttack());
            chaseSelector.Attach(chaseNode);

            return chaseSelector;
        }

        SequenceNode SetupPatrol()
        {
            SequenceNode mainSequence = new SequenceNode();
            float randomDelay = GameDevHelper.RandomInRange(patrol.randomDelayBounds);
            WaitNode wait = new WaitNode(randomDelay);
            PatrolNode patrolNode = new PatrolNode(patrol);
            ResetActorNode resetActor = new ResetActorNode(myActor);

            mainSequence.Attach(resetActor);
            mainSequence.Attach(wait);
            mainSequence.Attach(patrolNode);

            return mainSequence;
        }

        SequenceNode SetupAttack()
        {
            SequenceNode mainSequence = new SequenceNode();
            CheckDistanceNode inRange = new CheckDistanceNode(myActor, attack.attackRange.range);
            AttackNode attackNode = attack.GenerateNode();

            mainSequence.Attach(SetupGetTarget());
            mainSequence.Attach(inRange);
            mainSequence.Attach(attackNode);

            return mainSequence;
        }

        Selector SetupGetTarget()
        {
            Selector selector = new Selector();
            CheckTarget checkTarget = new CheckTarget(myActor);
            DetectTargetNode detectTarget = new DetectTargetNode(myActor, aggroRange.range);
            ReactionNode react = new ReactionNode(myActor);
            SequenceNode scanSequence = new SequenceNode();

            scanSequence.Attach(detectTarget);
            scanSequence.Attach(react);

            selector.Attach(checkTarget);
            selector.Attach(scanSequence);

            return selector;
        }

        SequenceNode SetupHealthCheck()
        {
            SequenceNode mainSequence = new SequenceNode();
            CheckHealthNode checkHealth = new CheckHealthNode(myActor, healthThreshold);
            RunAwayNode runAway = new RunAwayNode(myActor, sightRange.range);

            mainSequence.Attach(checkHealth);
            mainSequence.Attach(runAway);

            return mainSequence;
        }

        public override AINode MakeTree()
        {
            Selector topSelector = new Selector();
            topSelector.Attach(SetupHealthCheck());
            topSelector.Attach(SetupFight());
            topSelector.Attach(SetupPatrol());
            return topSelector;
        }
    }
}