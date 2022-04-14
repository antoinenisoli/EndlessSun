using DG.Tweening;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : Entity
{
    [Header("_NPC")]
    [SerializeField] AIGlobalBehavior myBehavior;
    public float attackRate = 1f;
    [SerializeField] float attackRange = 5f;
    [SerializeField] Transform destinationPoint;
    [SerializeField] SpriteRenderer detectIcon;
    [SerializeField] Transform healthBarPivot;
    [SerializeField] Transform healthBar;
    [SerializeField] ItemDrop[] loots;

    Vector3 detectIconBaseScale;
    float alpha;
    bool pushed;
    SpriteRenderer[] healthBarSprites;
    [HideInInspector] public Vector2 targetPos;
    protected List<Entity> aggressors = new List<Entity>();

    public override void Start()
    {
        base.Start();
        rb.isKinematic = true;
        detectIconBaseScale = detectIcon.transform.localScale;
        detectIcon.gameObject.SetActive(false);
        healthBarSprites = healthBar.GetComponentsInChildren<SpriteRenderer>();
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

    public override void Hit(float amount, Entity aggressor = null)
    {
        base.Hit(amount, aggressor);
        NewAgressor(aggressor);
        rb.isKinematic = false;
        healthBarPivot.DOScaleX((float)Health.CurrentValue / (float)Health.MaxValue, 0.3f);
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

    public override void ManageAnimations()
    {
        base.ManageAnimations();
        if (myBehavior)
            anim.SetFloat("Speed", myBehavior.GetVelocity());
        else
            anim.SetFloat("Speed", 0);
    }

    public override void Stun()
    {
        base.Stun();
        if (myBehavior)
            myBehavior.Stun();

        pushed = true;
    }

    public override void UnStun()
    {
        base.UnStun();
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
        if (pushed)
            return;

        base.Attack();
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

    Vector2 TargetPosition()
    {
        if (Target)
            return Target.transform.position;
        else
            return targetPos;
    }

    public bool IsMoving()
    {
        return rb.velocity.sqrMagnitude > 1.5f;
    }

    public override void Stop()
    {
        base.Stop();
        rb.velocity = Vector2.zero;      
    }

    void Flip(float otherX)
    {
        bool flip = transform.parent.position.x < otherX;
        transform.parent.rotation = flip ? Quaternion.identity : Quaternion.Euler(Vector2.up * -180);
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

    public override void DoUpdate()
    {
        base.DoUpdate();
        ManageHealthbars();
        Flip(TargetPosition().x);

        if (!Health.isDead && myBehavior)
            myBehavior.DoUpdate();
    }
}
