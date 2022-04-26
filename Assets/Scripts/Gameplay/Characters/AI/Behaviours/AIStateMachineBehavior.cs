using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NPC))]
public class AIStateMachineBehavior : AIGlobalBehavior
{
    protected SubBehavior behaviour;
    public AIState State;

    [Header("Patrol")]
    [SerializeField] Transform destinationPoint;
    public float minNextWaypointDistance = 3f;
    public float patrolStopDistance = 0.1f, chaseMinDistance = 2f, attackMinDistance = 2f;
    [SerializeField] Vector2 randomDelayBounds;
    public Vector2 randomPatrolRange;
    public ShowRectangleGizmo patrolGizmo;

    [Header("Detection")]
    public float reactTimer = 0.3f;
    public float aggroDistance = 20f, visionDistance = 50f;
    public ShowSphereGizmo aggroGizmo, visionGizmo;

    private void Start()
    {
        SetBehaviour(new Patrolling(this));
    }

    public void SetBehaviour(SubBehavior newBehaviour)
    {
        behaviour = newBehaviour;
        State = newBehaviour.State;
    }

    public float RandomDelay()
    {
        float delay = Random.Range(randomDelayBounds.x, randomDelayBounds.y);
        return delay;
    }

    public bool DetectTarget(out Entity target)
    {
        target = null;
        Collider2D[] colls = Physics2D.OverlapCircleAll(transform.position, aggroDistance);
        if (colls.Length > 0)
        {
            foreach (var item in colls)
            {
                Entity entity = item.GetComponent<Entity>();
                if (entity && !myNPC.SameTeam(entity))
                {
                    float distance = Vector2.Distance(entity.transform.position, transform.position);
                    if (distance < aggroDistance && !entity.Health.isDead)
                    {
                        target = entity;
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public Vector2 RandomPatrolPosition()
    {
        Vector2 vector = GameDevHelper.RandomVector(randomPatrolRange, transform.position);
        float dist = Vector2.Distance(vector, transform.position);
        int emergencyBreak = 1000;

        while (dist < minNextWaypointDistance && emergencyBreak > 0)
        {
            emergencyBreak--;
            vector = GameDevHelper.RandomVector(randomPatrolRange, transform.position);
            dist = Vector2.Distance(vector, transform.position);
        }

        return vector;
    }

    public override void Stun()
    {
        base.Stun();
        SetBehaviour(new Wait(this, 0.4f, AIState.Chasing));
    }

    public void Move(Vector3 targetPos)
    {
        myNPC.targetPos = targetPos;
        float distance = Vector2.Distance(targetPos, transform.position);
        float stopDistance = behaviour.State == AIState.Patrolling ? patrolStopDistance : chaseMinDistance;
        
        if (distance > stopDistance)
        {
            aiAgent.enabled = true;
            aiAgent.isStopped = false;
            destinationPoint.position = targetPos;
        }
        else
            Stop();
    }

    public override float GetVelocity()
    {
        return Mathf.Clamp01(aiAgent.velocity.sqrMagnitude);
    }

    public bool NearToTarget()
    {
        if (!myNPC.Target)
            return false;

        float distance = Vector2.Distance(myNPC.Target.transform.position, transform.position);
        return distance < chaseMinDistance;
    }

    public bool CanAttack()
    {
        if (!myNPC.Target)
            return false;

        float distance = Vector2.Distance(myNPC.Target.transform.position, transform.position);
        return distance < attackMinDistance;
    }

    public override void ReactToPlayer()
    {
        base.ReactToPlayer();
        SetBehaviour(new Reacting(this, reactTimer));
    }

    public bool KeepTargetInSight()
    {
        if (myNPC.Target)
        {
            float distance = Vector2.Distance(myNPC.Target.transform.position, transform.position);
            if (distance < visionDistance)
                return true;
        }

        return false;
    }

    public override float ComputeSpeed()
    {
        float speed = myNPC.AttributeList.Speed.value;
        if (State != AIState.Patrolling)
            return speed * 1.5f;

        return speed;
    }

    public override void DoUpdate()
    {
        if (aiAgent)
        {
            destinationSetter.target = destinationPoint;
            aiAgent.maxSpeed = myNPC.ComputeSpeed();
        }

        if (!myNPC.Health.isDead && behaviour != null)
            behaviour.Update();
    }
}
