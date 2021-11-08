using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Reacting : EnemyBehaviour
{
    public override EnemyState State => EnemyState.React;
    float timer;
    float delay;

    public Reacting(Entity target, Enemy myEnemy, float delay = 0) : base(target, myEnemy)
    {
        this.delay = delay;
        myEnemy.ReactToTarget();
    }

    public override void Update()
    {
        base.Update();
        timer += Time.deltaTime;
        if (timer > delay)
        {
            myEnemy.SetBehaviour(new Chasing(target, myEnemy));
        }
    }
}
