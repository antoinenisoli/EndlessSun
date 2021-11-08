using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Wait : EnemyBehaviour
{
    public override EnemyState State => EnemyState.React;
    float timer;
    float delay;
    EnemyState nextState;

    public Wait(Entity target, Enemy myEnemy, float delay, EnemyState nextState) : base(target, myEnemy)
    {
        this.delay = delay;
        this.nextState = nextState;
    }

    public override void Update()
    {
        base.Update();
        myEnemy.Stop();
        timer += Time.deltaTime;
        if (timer > delay)
        {
            switch (nextState)
            {
                case EnemyState.Patrolling:
                    myEnemy.SetBehaviour(new Patrolling(target, myEnemy));
                    break;
                case EnemyState.React:
                    myEnemy.SetBehaviour(new Reacting(target, myEnemy));
                    break;
                case EnemyState.Chasing:
                    myEnemy.SetBehaviour(new Chasing(target, myEnemy));
                    break;
                case EnemyState.Attacking:
                    myEnemy.SetBehaviour(new Attacking(target, myEnemy));
                    break;
            }
        }
    }
}
