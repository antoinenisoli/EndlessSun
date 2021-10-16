using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum EnemyState
{
    Patrolling,
    Chasing,
    Attacking,
}

[Serializable]
public abstract class EnemyBehaviour
{
    public abstract EnemyState State { get; }

    protected CharacterController player;
    protected Enemy myEnemy;

    protected EnemyBehaviour(CharacterController player, Enemy myEnemy)
    {
        this.player = player;
        this.myEnemy = myEnemy;
    }

    public virtual void Update()
    {
        if (!myEnemy || !player)
            return;
    }
}
