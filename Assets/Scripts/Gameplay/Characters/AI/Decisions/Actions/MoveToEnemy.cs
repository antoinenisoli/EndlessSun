using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToEnemy : AIAction
{
    protected override ActionState UpdateInterval(Actor actor)
    {
        return ActionState.InProgress;
    }
}
