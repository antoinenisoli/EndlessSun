using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum EnemyState
{
    Patrolling,
    React,
    Chasing,
    Attacking,
}

[Serializable]
public abstract class EnemyBehaviour
{
    public abstract EnemyState State { get; }

    protected Entity target;
    protected Enemy myEnemy;

    protected EnemyBehaviour(Entity target, Enemy myEnemy)
    {
        this.target = target;
        this.myEnemy = myEnemy;
    }

    public virtual void Update()
    {
        if (!myEnemy || !target)
            return;
    }
}
