using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Tilemaps;
using Pathfinding;

public class Enemy : Entity
{
    protected PlayerController2D Player => GameManager.Player;
    protected EnemyBehaviour behaviour = null;

    AIPath aiAgent;
    AIDestinationSetter destinationSetter;

    [Header("ENEMY")]
    [SerializeField] EnemyState currentState;
    [SerializeField] Transform healthBarPivot;
    [SerializeField] Transform healthBar;
    SpriteRenderer[] healthBarSprites;
    [SerializeField] GameObject xpPrefab;
    [SerializeField] int xpAmount = 25;
    [SerializeField] ItemDrop[] loots;
    float alpha;
    bool pushed;

    [Header("Patrol")]
    [SerializeField] Transform destinationPoint;
    public float minNextWaypointDistance = 3f;
    [SerializeField] float patrolStopDistance = 0.1f;
    [SerializeField] Vector2 randomDelayBounds;
    public Vector2 randomPatrolRange;
    public ShowRectangleGizmo patrolGizmo;

    [Header("Detection")]
    public float reactTimer = 0.3f;
    [SerializeField] SpriteRenderer detectIcon;
    Vector3 detectIconBaseScale;
    public float aggroDistance = 20f, visionDistance = 50f;
    public ShowSphereGizmo aggroGizmo, visionGizmo;

    [Header("Chase")]
    [SerializeField] float chaseMinDistance = 1f;

    [Header("Attack")]
    public float attackRate = 1f;
    [SerializeField] float attackRange = 5f;

    private void OnDrawGizmosSelected()
    {
        if (aggroGizmo)
            aggroGizmo.SetSize(aggroDistance);
        if (visionGizmo)
            visionGizmo.SetSize(visionDistance);
        if (patrolGizmo)
            patrolGizmo.SetSize(randomPatrolRange * 2);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!pushed)
            return;

        if (collision.CompareTag("ObstacleTilemap"))
        {
            Stop();
            Hit(5f);
        }
        else if (collision.CompareTag("GroundTilemap"))
            Death();
    }

    public override void Start()
    {
        base.Start();
        aiAgent = GetComponentInParent<AIPath>();
        destinationSetter = GetComponentInParent<AIDestinationSetter>();

        rb.isKinematic = true;
        detectIconBaseScale = detectIcon.transform.localScale;
        detectIcon.gameObject.SetActive(false);
        SetBehaviour(new Patrolling(this));
        healthBarSprites = healthBar.GetComponentsInChildren<SpriteRenderer>();
    }

    public override void Hit(float amount, Entity aggressor = null)
    {
        base.Hit(amount, aggressor);
        rb.isKinematic = false;
        healthBarPivot.DOScaleX((float)Health.CurrentValue / (float)Health.MaxValue, 0.3f);
    }

    public override void NewAgressor(Entity aggressor)
    {
        if (!aggressors.Contains(aggressor))
        {
            base.NewAgressor(aggressor);
            Stop();
            SetTarget(aggressor);
            SetBehaviour(new Reacting(this, reactTimer));
        }
    }

    public override void ManageAnimations()
    {
        base.ManageAnimations();
        anim.SetFloat("Speed", aiAgent.velocity.sqrMagnitude);
    }

    public override void Stun()
    {
        base.Stun();
        SetBehaviour(new Wait(this, 0.4f, EnemyState.Chasing));
        pushed = true;
    }

    public void UnStun()
    {
        pushed = false;
        rb.isKinematic = true;
    }

    public void LaunchAttack()
    {
        if (pushed)
            return;

        anim.SetTrigger("Attack");
    }

    public override void Attack()
    {
        if (pushed || !NearToTarget())
            return;

        base.Attack();
    }

    public override void Death()
    {
        base.Death();
        SpawnXP();
    }

    public void ReactToTarget()
    {
        anim.SetTrigger("React");
        detectIcon.gameObject.SetActive(true);
        detectIcon.transform.localScale = detectIconBaseScale;
        detectIcon.DOFade(1, 0);

        Sequence tweenSequence = DOTween.Sequence();
        tweenSequence.Append(detectIcon.transform.DOScale(detectIconBaseScale * 1.2f, 0.3f));
        tweenSequence.Append(detectIcon.DOFade(0, 0.3f));
        tweenSequence.OnComplete(() =>
        {
            detectIcon.gameObject.SetActive(false);
        });
    }

    public float RandomDelay()
    {
        float delay = Random.Range(randomDelayBounds.x, randomDelayBounds.y);
        return delay;
    }

    public Vector2 RandomPatrolPosition()
    {
        Vector2 vector = GameDevHelper.RandomVector(randomPatrolRange, transform.position);
        float dist = Vector2.Distance(vector, transform.position);
        while (dist < minNextWaypointDistance)
        {
            vector = GameDevHelper.RandomVector(randomPatrolRange, transform.position);
            dist = Vector2.Distance(vector, transform.position);
        }

        return vector;
    }

    void SpawnXP()
    {
        foreach (var item in loots)
        {
            for (int i = 0; i < item.dropCount; i++)
            {
                float random = Random.value;
                if (random < item.prob)
                    Instantiate(item.itemPrefab, transform.position, Quaternion.identity);
            }
        }

        int result = xpAmount / xpPrefab.GetComponent<XPItem>().xpAmount;
        for (int i = 0; i < result; i++)
            Instantiate(xpPrefab, transform.position, Quaternion.identity);
    }

    public bool KeepTargetInSight()
    {
        if (Target)
        {
            float distance = Vector2.Distance(Target.transform.position, transform.position);
            if (distance < visionDistance)
                return true;
        }

        return false;
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
                if (entity && entity.myTeam != myTeam)
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

    public bool NearToTarget()
    {
        if (!Target)
            return false;

        float distance = Vector2.Distance(Target.transform.position, transform.position);
        return distance < chaseMinDistance;
    }

    public void SetBehaviour(EnemyBehaviour newBehaviour)
    {
        behaviour = newBehaviour;
    }

    public bool isMoving()
    {
        return rb.velocity.sqrMagnitude > 1.5f;
    }

    public void Stop()
    {
        rb.velocity = Vector2.zero;
        aiAgent.isStopped = true;
        aiAgent.enabled = false;
    }

    void Flip(float otherX)
    {
        bool flip = transform.parent.position.x < otherX;
        transform.parent.rotation = flip ? Quaternion.identity : Quaternion.Euler(Vector2.up * -180);
    }

    public void Move(Vector3 targetPos)
    {
        Flip(targetPos.x);
        float distance = Vector2.Distance(targetPos, transform.position);
        float stopDistance = behaviour.State == EnemyState.Patrolling ? patrolStopDistance : chaseMinDistance;

        if (distance > stopDistance)
        {
            aiAgent.enabled = true;
            aiAgent.isStopped = false;
            destinationPoint.position = targetPos;
        }
        else
            Stop();
    }

    void ManageHealthbars()
    {
        alpha = Mathf.Lerp(alpha, Health.CurrentValue < Health.MaxValue ? 1 : 0, 15 * Time.deltaTime);
        foreach (var item in healthBarSprites)
        {
            Color col = item.color;
            col.a = alpha;
            item.color = col;
        }
    }

    public override float ComputeSpeed()
    {
        if (behaviour.State == EnemyState.Patrolling)
            return walkSpeed;
        else
            return runSpeed;
    }

    public override void Update()
    {
        base.Update();
        ManageHealthbars();
        currentState = behaviour.State;

        if (aiAgent)
        {
            destinationSetter.target = destinationPoint;
            aiAgent.maxSpeed = ComputeSpeed();
        }

        if (Target)
        {
            Flip(Target.transform.position.x);
        }

        if (!Health.isDead)
            behaviour.Update();
    }
}
