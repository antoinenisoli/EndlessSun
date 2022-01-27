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

    public Wait(Enemy myEnemy, float delay, EnemyState nextState) : base(myEnemy)
    {
        this.delay = delay;
        this.nextState = nextState;
        myEnemy.Stop();
    }

    public override void Update()
    {
        base.Update();
        timer += Time.deltaTime;
        if (timer > delay)
        {
            switch (nextState)
            {
                case EnemyState.Patrolling:
                    myEnemy.SetBehaviour(new Patrolling(myEnemy));
                    break;
                case EnemyState.React:
                    myEnemy.SetBehaviour(new Reacting(myEnemy));
                    break;
                case EnemyState.Chasing:
                    myEnemy.SetBehaviour(new Chasing(myEnemy));
                    break;
                case EnemyState.Attacking:
                    myEnemy.SetBehaviour(new Attacking(myEnemy));
                    break;
            }
        }
    }
}
