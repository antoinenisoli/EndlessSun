﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum AIState
{
    Patrolling,
    React,
    Chasing,
    Attacking,
    Waiting,
}

namespace CustomAI
{
    [Serializable]
    public abstract class SubBehavior
    {
        public abstract AIState State { get; }
        public AIStateMachineBehavior behavior;
        protected NPC myNPC => behavior.myActor as NPC;

        public SubBehavior(AIStateMachineBehavior behavior)
        {
            this.behavior = behavior;
            myNPC.UnStun();
        }

        public virtual void Gizmos() { }

        public virtual void Update() { }
    }
}
