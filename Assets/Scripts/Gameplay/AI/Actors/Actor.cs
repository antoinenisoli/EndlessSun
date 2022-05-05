using CustomAI;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Actor : Entity
{
    [Header(nameof(Actor))]
    public AIState State;
    public AIPath aiAgent;
    public AIDestinationSetter destinationSetter;
    public Transform destinationPoint;
    public AIGlobalBehavior myBehavior;
    public bool isReacting;
    [SerializeField] protected float reactDuration = 0.5f;

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

    public void ResetActor()
    {
        Stop();
        if (MainTarget)
            MainTarget.SetTarget(null);

        SetTarget(null);
    }

    public Vector2 TargetPosition()
    {
        if (MainTarget)
            return MainTarget.transform.position;
        else
        {
            return destinationPoint.position;
        }
    }

    public void Move(Vector2 targetPos, float speedMultiplier = 1)
    {
        aiAgent.enabled = true;
        aiAgent.isStopped = false;
        aiAgent.maxSpeed = BaseSpeed() * speedMultiplier;

        destinationPoint.position = SampledPosition(targetPos);
    }

    Vector2 SampledPosition(Vector2 pos)
    {
        if (GridManager.Instance)
            if (GridManager.Instance.SamplePosition(pos, 2f, out Vector2 sampledPos))
                return sampledPos;

        return pos;
    }

    public override void Stop()
    {
        base.Stop();
        aiAgent.isStopped = true;
        aiAgent.enabled = false;
        destinationPoint.position = transform.position;
    }

    public bool CheckDistance(float minDistance)
    {
        if (!MainTarget)
            return false;

        float dist = Vector2.Distance(MainTarget.transform.position, transform.position);
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
