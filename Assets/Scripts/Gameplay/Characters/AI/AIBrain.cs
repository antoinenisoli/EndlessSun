using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AIBrain
{
    List<AIAction> actions = new List<AIAction>();
    int currentAction;
    Actor actor;

    public AIBrain(Actor actor)
    {
        this.actor = actor;
        actions.Add(new WaitAction());
        actions.Add(new MoveToEnemy());
        actions.Add(new ActionAttack());
    }

    public void Update()
    {
        AIAction action = actions[currentAction];
        if (action.Update(actor) == AIAction.ActionState.Finished)
        {
            currentAction++;
            currentAction %= actions.Count;
        }
    }
}
