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
