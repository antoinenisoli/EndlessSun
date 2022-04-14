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

    protected Enemy myEnemy;

    protected EnemyBehaviour(Enemy myEnemy)
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
