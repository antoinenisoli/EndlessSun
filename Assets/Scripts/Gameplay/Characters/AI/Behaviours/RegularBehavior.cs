using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegularBehavior : AIBehavior
{
    protected StateMachineBehavior behaviour = null;

    AIPath aiAgent;
    AIDestinationSetter destinationSetter;
    [SerializeField] AIState currentState;

    [Header("Patrol")]
    [SerializeField] Transform destinationPoint;
    public float minNextWaypointDistance = 3f;
    [SerializeField] float patrolStopDistance = 0.1f;
    [SerializeField] Vector2 randomDelayBounds;
    public Vector2 randomPatrolRange;
    public ShowRectangleGizmo patrolGizmo;

    [Header("Detection")]
    public float reactTimer = 0.3f;
    public float aggroDistance = 20f, visionDistance = 50f;
    public ShowSphereGizmo aggroGizmo, visionGizmo;
    Vector3 detectIconBaseScale;

    [Header("Chase")]
    [SerializeField] float chaseMinDistance = 1f;

    [Header("Attack")]
    public float attackRate = 1f;
    [SerializeField] float attackRange = 5f;

    private void Start()
    {
        SetBehaviour(new Patrolling(this));
    }

    public void SetBehaviour(StateMachineBehavior newBehaviour)
    {
        behaviour = newBehaviour;
    }

    public override void Stun()
    {
        base.Stun();
        SetBehaviour(new Wait(this, 0.4f, AIState.Chasing));
    }

    public override void React()
    {
        base.React();
        SetBehaviour(new Reacting(this, reactTimer));
    }

    public void Move(Vector3 targetPos)
    {
        float distance = Vector2.Distance(targetPos, transform.position);
        float stopDistance = behaviour.State == AIState.Patrolling ? patrolStopDistance : chaseMinDistance;

        if (distance > stopDistance)
        {
            aiAgent.enabled = true;
            aiAgent.isStopped = false;
            destinationPoint.position = targetPos;
        }
        else
            myEntity.Stop();
    }

    public bool NearToTarget()
    {
        if (!myEntity.Target)
            return false;

        float distance = Vector2.Distance(myEntity.Target.transform.position, transform.position);
        return distance < chaseMinDistance;
    }

    public override void DoUpdate()
    {
        currentState = behaviour.State;

        if (aiAgent)
        {
            destinationSetter.target = destinationPoint;
            aiAgent.maxSpeed = myEntity.ComputeSpeed();
        }

        if (!myEntity.Health.isDead)
            behaviour.Update();
    }
}
