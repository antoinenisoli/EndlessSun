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

    protected Enemy myEnemy;

    protected StateMachineBehavior(Enemy myEnemy)
    {
        myEnemy.UnStun();
        this.myEnemy = myEnemy;
    }

    public virtual void Gizmos() { }

    public virtual void Update()
    {
        if (!myEnemy)
            return;
    }
}
