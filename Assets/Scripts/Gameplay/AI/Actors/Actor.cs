﻿using CustomAI;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Actor : Entity
{
    [Header("AI")]
    public Entity Target;
    public AIState State;
    public AIPath aiAgent;
    public AIDestinationSetter destinationSetter;
    public Transform destinationPoint;
    public AIGlobalBehavior myBehavior;
    public List<Entity> aggressors = new List<Entity>();
    public bool isReacting;
    [SerializeField] protected float reactDuration = 0.5f;

    public override float ComputeSpeed()
    {
        return myBehavior.ComputeSpeed();
    }

    public virtual void SetTarget(Entity target)
    {
        if (Target != null)
            oldTarget = Target;

        Target = target;
        if (!target && Target && aggressors.Contains(Target))
            aggressors.Remove(Target);
    }

    public void NewAgressor(Entity aggressor)
    {
        if (!aggressors.Contains(aggressor))
        {
            aggressors.Add(aggressor);
            Stop();
            SetTarget(aggressor);
        }
    }

    public virtual void ReactToTarget()
    {
        Stop();
        StartCoroutine(Reaction());
    }

    IEnumerator Reaction()
    {
        anim.SetTrigger("React");
        isReacting = true;
        yield return new WaitForSeconds(reactDuration);
        isReacting = false;
    }

    public void Move(Vector2 targetPos)
    {
        aiAgent.enabled = true;
        aiAgent.isStopped = false;
        destinationPoint.position = targetPos;
    }

    public bool CheckDistance(float minDistance)
    {
        if (!Target)
            return false;

        float dist = Vector2.Distance(Target.transform.position, transform.position);
        return dist < minDistance;
    }

    public override void DoUpdate()
    {
        base.DoUpdate();
        destinationSetter.target = destinationPoint;
        if (!Health.isDead && myBehavior && !isReacting)
            myBehavior.DoUpdate();
        else
            Stop();
    }
}
