using CustomAI;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Actor : Entity
{
    [Header("AI")]
    public AIState State;
    public AIPath aiAgent;
    public AIDestinationSetter destinationSetter;
    public Transform destinationPoint;
    [SerializeField] protected AIGlobalBehavior myBehavior;
    public List<Entity> aggressors = new List<Entity>();

    public override float ComputeSpeed()
    {
        return myBehavior.ComputeSpeed();
    }

    public void NewAgressor(Entity aggressor)
    {
        if (!aggressors.Contains(aggressor))
        {
            aggressors.Add(aggressor);
            Stop();
            SetTarget(aggressor);
            myBehavior.ReactToPlayer();
        }
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
}
