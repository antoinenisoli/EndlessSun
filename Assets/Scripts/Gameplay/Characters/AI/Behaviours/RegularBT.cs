using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CustomAI.BehaviorTree
{
    [System.Serializable]
    public struct DistanceCheck
    {
        public float range;
        public Color color;

        public DistanceCheck(float range, Color color)
        {
            this.range = range;
            this.color = color;
        }

#if UNITY_EDITOR
        public void Draw(Actor actor)
        {
            Handles.color = color;
            GUI.color = color;

            Vector3 pos = actor.transform.position;
            range =
                Handles.ScaleValueHandle(
                    range,
                    pos + actor.transform.right * range,
                    actor.transform.rotation,
                    5,
                    Handles.ConeHandleCap,
                    1
                );

            Handles.DrawWireDisc(pos, actor.transform.forward, range);
            Handles.Label(
              pos + (range + 0.5f) * Vector3.right,
              range.ToString("0.0")
            );
        }
#endif

    }

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
            SequenceNode sequence = new SequenceNode();
            DetectTargetNode detectTarget = new DetectTargetNode(myActor, aggroRange.range);
            TargetInSightNode inSightNode = new TargetInSightNode(myActor, sightRange.range);
            ChaseNode chaseNode = new ChaseNode(myActor, chaseRange.range);
            ReactionNode react = new ReactionNode(reactDuration, myActor);

            sequence.Attach(detectTarget);
            sequence.Attach(react);
            sequence.Attach(inSightNode);
            sequence.Attach(chaseNode);
            return sequence;
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
            return sequence;
        }

        public override AINode MakeTree()
        {
            Selector topSelector = new Selector();
            //topSelector.Attach(SetupAttack());
            topSelector.Attach(SetupChase());
            topSelector.Attach(SetupPatrol());
            return topSelector;
        }
    }
}