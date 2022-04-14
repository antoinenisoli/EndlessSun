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
    
    public Wait(RegularBehavior behavior, float delay, AIState nextState) : base(behavior)
    {
        this.delay = delay;
        this.nextState = nextState;
        behavior.myEntity.Stop();
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
                    behavior.SetBehaviour(new Patrolling(behavior));
                    break;
                case AIState.React:
                    behavior.SetBehaviour(new Reacting(behavior));
                    break;
                case AIState.Chasing:
                    behavior.SetBehaviour(new Chasing(behavior));
                    break;
                case AIState.Attacking:
                    behavior.SetBehaviour(new Attacking(behavior));
                    break;
            }
        }
    }
}
