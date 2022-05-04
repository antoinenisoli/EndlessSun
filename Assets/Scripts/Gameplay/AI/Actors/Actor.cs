using CustomAI;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Actor : Entity
{
    [Header(nameof(Actor))]
    public Entity Target;
    public AIState State;
    public AIPath aiAgent;
    public AIDestinationSetter destinationSetter;
    public Transform destinationPoint;
    public AIGlobalBehavior myBehavior;
    public bool isReacting;
    [SerializeField] protected float reactDuration = 0.5f;

    public List<Entity> aggressors = new List<Entity>();

    public virtual void SetTarget(Entity target)
    {
        if (Target != null)
            oldTarget = Target;

        if (!target && aggressors.Contains(Target))
            aggressors.Remove(Target);

        Target = target;
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

    public void ResetActor()
    {
        Stop();
        SetTarget(null);
    }

    IEnumerator Reaction()
    {
        anim.SetTrigger("React");
        isReacting = true;
        yield return new WaitForSeconds(reactDuration);
        isReacting = false;
    }

    public Vector2 TargetPosition()
    {
        if (Target)
            return Target.transform.position;
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
        rb.velocity = Vector2.zero;
    }

    public bool CheckDistance(float minDistance)
    {
        if (!Target)
            return false;

        float dist = Vector2.Distance(Target.transform.position, transform.position);
        return dist < minDistance;
    }

    public override void TakeDamages(float amount, Entity aggressor = null, Vector2 impactPoint = default)
    {
        rb.isKinematic = false;
        if (aggressor)
        {
            NewAgressor(aggressor);
            if (aggressor.BalanceDraw(this))
            {
                Vector2 direction = (impactPoint - (Vector2)transform.position).normalized;
                Push(direction * pushForce);
            }
        }

        base.TakeDamages(amount, aggressor);
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
