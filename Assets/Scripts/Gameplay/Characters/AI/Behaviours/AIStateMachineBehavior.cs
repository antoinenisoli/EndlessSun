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
            actor.State = newBehaviour.State;
        }

        public override void Stun()
        {
            base.Stun();
            SetBehaviour(new Wait(this, 0.4f, AIState.Chasing));
        }

        public void Move(Vector3 targetPos)
        {
            float distance = Vector2.Distance(targetPos, transform.position);
            float stopDistance = behaviour.State == AIState.Patrolling ? patrol.stopDistance : patrol.chaseMinDistance;

            if (distance > stopDistance)
                actor.Move(targetPos);
            else
                Stop();
        }

        public override float GetVelocity()
        {
            return Mathf.Clamp01(actor.aiAgent.velocity.sqrMagnitude);
        }

        public override void ReactToPlayer()
        {
            base.ReactToPlayer();
            SetBehaviour(new Reacting(this, reactTimer));
        }

        public bool KeepTargetInSight()
        {
            if (actor.Target)
            {
                float distance = Vector2.Distance(actor.Target.transform.position, transform.position);
                if (distance < visionDistance)
                    return true;
            }

            return false;
        }

        public override float ComputeSpeed()
        {
            float speed = actor.AttributeList.Speed.value;
            if (actor.State != AIState.Patrolling)
                return speed * 1.5f;

            return speed;
        }

        public override void DoUpdate()
        {
            if (actor.aiAgent)
            {
                actor.destinationSetter.target = actor.destinationPoint;
                actor.aiAgent.maxSpeed = actor.ComputeSpeed();
            }

            if (!actor.Health.isDead && behaviour != null)
                behaviour.Update();
        }
    }
}