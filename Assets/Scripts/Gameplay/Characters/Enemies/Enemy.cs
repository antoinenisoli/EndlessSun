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
    [SerializeField] ShowRectangleGizmo gizmo;
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
    [SerializeField] Vector2 randomDelayBounds;
    [SerializeField] Vector2 randomPatrolRange;
    Vector2 startPosition;

    [Header("Detection")]
    public float reactTimer = 0.3f;
    [SerializeField] SpriteRenderer detectIcon;
    Vector3 detectIconBaseScale;
    [SerializeField] float visionDistance = 20f;

    [Header("Chase")]
    [SerializeField] float minDistance = 1f;

    [Header("Attack")]
    public float attackRate = 1f;
    [SerializeField] float attackRange = 5f;

    private void OnDrawGizmosSelected()
    {
        if (gizmo)
            gizmo.SetSize(randomPatrolRange);
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
        {            
            Death();
        }
    }

    public override void Start()
    {
        base.Start();
        aiAgent = GetComponentInParent<AIPath>();
        destinationSetter = GetComponentInParent<AIDestinationSetter>();

        rb.isKinematic = true;
        detectIconBaseScale = detectIcon.transform.localScale;
        detectIcon.gameObject.SetActive(false);
        startPosition = transform.position;
        behaviour = new Patrolling(this);
        healthBarSprites = healthBar.GetComponentsInChildren<SpriteRenderer>();
    }

    public override void Hit(float amount)
    {
        rb.isKinematic = false;
        base.Hit(amount);
        healthBarPivot.DOScaleX((float)Health.CurrentValue / (float)Health.MaxValue, 0.3f);
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
        Destroy(gameObject, 2f);
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

    public Vector2 RandomPatrolPosition() => GameDevHelper.RandomVector(randomPatrolRange, transform.position);

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

    public bool DetectTargets()
    {
        Collider2D[] colls = Physics2D.OverlapCircleAll(transform.position, visionDistance, targetLayer);
        if (colls.Length > 0)
        {
            foreach (var item in colls)
            {
                Entity entity = item.GetComponent<Entity>();
                if (entity)
                {
                    float distance = Vector2.Distance(entity.transform.position, transform.position);
                    if (distance < visionDistance && !entity.Health.isDead)
                    {
                        Target = entity;
                        return true;
                    }
                }
            }
        }
        else
            Target = null;

        return false;
    }

    public bool NearToTarget()
    {
        if (!Target)
            return false;

        float distance = Vector2.Distance(Target.transform.position, transform.position);
        return distance < minDistance;
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
        aiAgent.enabled = false;
    }

    public void Move(Vector3 targetPos)
    {
        spr.flipX = transform.position.x > targetPos.x;
        float distance = Vector2.Distance(targetPos, transform.position);
        if (distance > minDistance)
        {
            aiAgent.enabled = true;
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

    public override void Update()
    {
        base.Update();
        ManageHealthbars();
        if (aiAgent)
        {
            destinationSetter.target = destinationPoint;
        }

        if (Target)
            spr.flipX = transform.position.x > Target.transform.position.x;

        if (!Health.isDead)
            behaviour.Update();
    }
}
