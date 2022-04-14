using System;
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
}

[Serializable]
public abstract class StateMachineBehavior
{
    public abstract AIState State { get; }
    public RegularBehavior behavior;
    protected Entity myEntity;

    public StateMachineBehavior(RegularBehavior behavior)
    {
        this.behavior = behavior;
        myEntity = behavior.myEntity;
        myEntity.UnStun();
    }

    public virtual void Gizmos() { }

    public virtual void Update() { }
}
