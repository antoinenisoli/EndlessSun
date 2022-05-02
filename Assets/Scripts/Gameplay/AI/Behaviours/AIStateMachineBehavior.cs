using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomAI
{
    [RequireComponent(typeof(NPC))]
    public class AIStateMachineBehavior : AIGlobalBehavior
    {
        protected SubBehavior behaviour;

        [Header("Patrol")]
        public PatrolData patrol;

        [Header("Detection")]
        public float reactTimer = 0.3f;
        public float aggroDistance = 20f, visionDistance = 50f;
        public ShowSphereGizmo aggroGizmo, visionGizmo;

        private void Start()
        {
            SetBehaviour(new Patrolling(this));
        }

        public void SetBehaviour(SubBehavior newBehaviour)
        {
            behaviour = newBehaviour;
            myActor.State = newBehaviour.State;
        }

        public bool DetectTarget(out Entity target)
        {
            target = null;
            Collider2D[] colls = Physics2D.OverlapCircleAll(transform.position, aggroDistance);
            if (colls.Length > 0)
            {
                foreach (var item in colls)
                {
                    Entity entity = item.GetComponent<Entity>();
                    if (entity && !myActor.SameTeam(entity))
                    {
                        float distance = Vector2.Distance(entity.transform.position, transform.position);
                        if (distance < aggroDistance && !entity.Health.isDead)
                        {
                            target = entity;
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public override void Stun()
        {
            base.Stun();
            SetBehaviour(new Wait(this, 0.4f, AIState.Chasing));
        }

        public void Move(Vector3 targetPos)
        {
            float distance = Vector2.Distance(targetPos, transform.position);
            if (distance > myActor.aiAgent.endReachedDistance)
                myActor.Move(targetPos);
            else
                Stop();
        }

        public override float GetVelocity()
        {
            return Mathf.Clamp01(myActor.aiAgent.velocity.sqrMagnitude);
        }

        public override void ReactToTarget()
        {
            base.ReactToTarget();
            SetBehaviour(new Reacting(this, reactTimer));
        }

        public bool KeepTargetInSight()
        {
            if (myActor.Target)
            {
                float distance = Vector2.Distance(myActor.Target.transform.position, transform.position);
                if (distance < visionDistance)
                    return true;
            }

            return false;
        }

        public override float ComputeSpeed()
        {
            float speed = myActor.AttributeList.Speed.value;
            if (myActor.State != AIState.Patrolling)
                return speed * 1.5f;

            return speed;
        }

        public override void DoUpdate()
        {
            if (myActor.aiAgent)
            {
                myActor.destinationSetter.target = myActor.destinationPoint;
                myActor.aiAgent.maxSpeed = myActor.ComputeSpeed();
            }

            if (!myActor.Health.isDead && behaviour != null)
                behaviour.Update();
        }
    }
}