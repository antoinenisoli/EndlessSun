using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomAI
{
    public enum AIState
    {
        Patrolling,
        React,
        Chasing,
        Attacking,
        Waiting,
    }

    [RequireComponent(typeof(NPC))]
    public abstract class AIGlobalBehavior : MonoBehaviour
    {
        [HideInInspector] public Actor myActor;

        public virtual void Awake()
        {
            myActor = GetComponent<Actor>();
        }

        public abstract void DoUpdate();

        public virtual void Attack() { }
        public virtual void Stun() { }
        public virtual void ReactToTarget() { myActor.ReactToTarget(); }
        public virtual float GetVelocity() { return 0; }
    }
}