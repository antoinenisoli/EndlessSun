using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Enemy : Entity
{
    protected PlayerController2D Player => GameManager.Player;
    protected EnemyBehaviour behaviour = null;

    [Header("ENEMY")]
    [SerializeField] Transform healthBarPivot;
    [SerializeField] Transform healthBar;
    SpriteRenderer[] healthBarSprites;
    [SerializeField] GameObject xpPrefab;
    [SerializeField] int xpAmount = 25;
    [SerializeField] ItemDrop[] loots;
    float alpha;

    [Header("Patrol")]
    [SerializeField] Vector2 randomDelayBounds;
    [SerializeField] Vector2 randomPatrolRange;
    Vector2 startPosition;

    [Header("Detection")]
    [SerializeField] float visionDistance = 20f;

    [Header("Chase")]
    [SerializeField] float minDistance = 1f;

    [Header("Attack")]
    public float attackRate = 1f;
    [SerializeField] float attackRange = 5f;

    private void OnDrawGizmosSelected()
    {
        Color c = Color.red;
        Gizmos.color = c;
        Gizmos.DrawWireSphere(transform.position, visionDistance);

        c.r -= 50f;
        Gizmos.color = c;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        c = Color.green;
        Gizmos.color = c;
        Gizmos.DrawWireSphere(startPosition, randomPatrolRange.y);
        c.g -= 25f;
        Gizmos.DrawWireSphere(startPosition, randomPatrolRange.x);
    }

    public override void Start()
    {
        base.Start();
        startPosition = transform.position;
        behaviour = new Patrolling(Player, this);
        healthBarSprites = healthBar.GetComponentsInChildren<SpriteRenderer>();
    }

    public override void Hit(float amount, Vector2 force = default)
    {
        base.Hit(amount, force);
        stunned = true;
        StartCoroutine(Unstun());
        healthBarPivot.DOScaleX((float)health.CurrentValue / (float)health.MaxValue, 0.3f);
    }

    public virtual void Attack(Entity target)
    {
        anim.SetTrigger("Attack");

    }

    IEnumerator Unstun()
    {
        yield return new WaitForSeconds(0.45f);
        stunned = false;
    }

    public override void Death()
    {
        base.Death();
        SpawnXP();
        Destroy(gameObject, 2f);
    }

    public float RandomDelay()
    {
        float delay = Random.Range(randomDelayBounds.x, randomDelayBounds.y);
        return delay;
    }

    public Vector3 RandomPatrolPosition()
    {
        float randomRange = Random.Range(randomPatrolRange.x, randomPatrolRange.y);
        Vector2 randomPos = Random.insideUnitCircle * randomRange;
        return startPosition + randomPos;
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

    public bool DetectTargets(Vector3 targetPos)
    {
        float distance = Vector2.Distance(targetPos, transform.position);
        return distance < visionDistance;
    }

    public bool NearToTarget(Vector3 targetPos)
    {
        float distance = Vector2.Distance(targetPos, transform.position);
        return distance < minDistance;
    }

    public void SetBehaviour(EnemyBehaviour newBehaviour)
    {
        behaviour = newBehaviour;
        if (newBehaviour.State == EnemyState.Chasing)
            anim.SetTrigger("React");
    }

    public bool isMoving()
    {
        return rb.velocity.magnitude > 1f;
    }

    public void Move(Vector3 targetPos)
    {
        spr.flipX = transform.position.x > targetPos.x;
        float distance = Vector2.Distance(targetPos, transform.position);
        if (distance > minDistance)
            rb.AddForce((targetPos - transform.position).normalized * baseSpeed * Time.deltaTime);
    }

    void ManageHealthbars()
    {
        alpha = Mathf.Lerp(alpha, health.CurrentValue < health.MaxValue ? 1 : 0, 15 * Time.deltaTime);
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
        if (GameManager.Player && !health.isDead)
            behaviour.Update();
    }
}
