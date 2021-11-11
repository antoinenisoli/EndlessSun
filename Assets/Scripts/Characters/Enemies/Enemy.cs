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
        Color c = Color.red;
        Gizmos.color = c;
        Gizmos.DrawWireSphere(transform.position, visionDistance);

        c.r -= 50f;
        Gizmos.color = c;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        c = Color.green;
        Gizmos.color = c;
        if (startPosition == Vector2.zero)
            Gizmos.DrawWireSphere(transform.position, randomPatrolRange.y);
        else
            Gizmos.DrawWireSphere(startPosition, randomPatrolRange.y);

        c.g -= 25f;
        if (startPosition == Vector2.zero)
            Gizmos.DrawWireSphere(transform.position, randomPatrolRange.x);
        else
            Gizmos.DrawWireSphere(startPosition, randomPatrolRange.x);
    }

    public override void Start()
    {
        base.Start();
        detectIconBaseScale = detectIcon.transform.localScale;
        detectIcon.gameObject.SetActive(false);
        startPosition = transform.position;
        behaviour = new Patrolling(Player, this);
        healthBarSprites = healthBar.GetComponentsInChildren<SpriteRenderer>();
    }

    public override void Hit(float amount, Vector2 force = default)
    {
        base.Hit(amount, force);
        healthBarPivot.DOScaleX((float)Health.CurrentValue / (float)Health.MaxValue, 0.3f);
        if (force.sqrMagnitude > 0)
        {
            stunned = true;
            StartCoroutine(Unstun());
        }
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
    }

    public bool isMoving()
    {
        return rb.velocity.sqrMagnitude > 1.5f;
    }

    public void Stop()
    {
        rb.velocity = Vector2.zero;
    }

    public void Move(Vector3 targetPos, bool chase = false)
    {
        spr.flipX = transform.position.x > targetPos.x;
        float distance = Vector2.Distance(targetPos, transform.position);
        float speed = chase ? runSpeed : walkSpeed;
        if (distance > minDistance)
        {
            //rb.AddForce((targetPos - transform.position).normalized * speed * Time.deltaTime);
            rb.velocity = (targetPos - transform.position).normalized * speed;
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
        if (GameManager.Player && !Health.isDead)
            behaviour.Update();
    }
}
