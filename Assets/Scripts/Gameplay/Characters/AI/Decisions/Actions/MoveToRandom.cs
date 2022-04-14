using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToRandom : AIAction
{
    public float rangeMaX = 5;

    public override void Start(Actor actor)
    {
        base.Start(actor);
        Vector3 circle = Random.insideUnitCircle;
        actor.MoveTo(actor.startposition + circle * rangeMaX);
    }

    protected override ActionState UpdateInterval(Actor actor)
    {
        if (!actor.IsMoving)
        {
            return ActionState.Finished;
        }

        return ActionState.InProgress;
    }
}
