using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomAI
{
    [RequireComponent(typeof(Actor))]
    public abstract class AIGlobalBehavior : MonoBehaviour
    {
        [HideInInspector] public Actor myActor;

        public virtual void Awake()
        {
            myActor = GetComponent<Actor>();
        }

        public virtual void Stop()
        {
            myActor.aiAgent.isStopped = true;
            myActor.aiAgent.enabled = false;
        }

        public abstract void DoUpdate();

        public virtual void Stun() { }
        public virtual void ReactToTarget() { }

        public virtual float GetVelocity() { return 0; }
        public virtual float ComputeSpeed() { return 0; }
    }
}