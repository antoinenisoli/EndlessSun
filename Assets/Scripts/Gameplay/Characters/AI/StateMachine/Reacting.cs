using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Reacting : StateMachineBehavior
{
    public override AIState State => AIState.React;
    float timer;
    float delay;

    public Reacting(RegularBehavior behavior, float delay = 0f) : base(behavior)
    {
        this.delay = delay;
        behavior.React();
    }

    public override void Update()
    {
        base.Update();
        timer += Time.deltaTime;
        if (timer > delay)
            behavior.SetBehaviour(new Chasing(behavior));
    }
}
