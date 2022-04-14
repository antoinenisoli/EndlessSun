using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Wait : StateMachineBehavior
{
    public override AIState State => AIState.React;
    float timer;
    float delay;
    AIState nextState;

    public Wait(Enemy myEnemy, float delay, AIState nextState) : base(myEnemy)
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
                case AIState.Patrolling:
                    myEnemy.SetBehaviour(new Patrolling(myEnemy));
                    break;
                case AIState.React:
                    myEnemy.SetBehaviour(new Reacting(myEnemy));
                    break;
                case AIState.Chasing:
                    myEnemy.SetBehaviour(new Chasing(myEnemy));
                    break;
                case AIState.Attacking:
                    myEnemy.SetBehaviour(new Attacking(myEnemy));
                    break;
            }
        }
    }
}
