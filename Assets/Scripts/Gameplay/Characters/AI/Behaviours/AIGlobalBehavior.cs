using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomAI
{
    [RequireComponent(typeof(Actor))]
    public abstract class AIGlobalBehavior : MonoBehaviour
    {
        [HideInInspector] public Actor actor;

        public virtual void Awake()
        {
            actor = GetComponent<Actor>();
        }

        public virtual void Stop()
        {
            actor.aiAgent.isStopped = true;
            actor.aiAgent.enabled = false;
        }

        public abstract void DoUpdate();

        public virtual void Stun() { }
        public virtual void ReactToPlayer() { }

        public virtual float GetVelocity() { return 0; }
        public virtual float ComputeSpeed() { return 0; }
    }
}