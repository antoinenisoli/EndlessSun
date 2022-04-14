using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionAttack : AIAction
{
    protected override ActionState UpdateInterval(Actor actor)
    {
        string attackState = "Attack";
        if (!actor.IsPlayingAction(attackState))
            actor.SetState(attackState);

        return ActionState.Finished;
    }
}
