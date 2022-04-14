using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitAction : AIAction
{
    Vector2 timerRandom = new Vector2(1, 3);

    public override void Start(Actor actor)
    {
        base.Start(actor);
        float randomTimer = Random.Range(timerRandom.x, timerRandom.y);
    }

    protected override ActionState UpdateInterval(Actor actor)
    {
        throw new System.NotImplementedException();
    }
}
