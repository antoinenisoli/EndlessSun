using CustomAI;
using DG.Tweening;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : Actor
{
    [Header(nameof(NPC))]
    public float attackRate = 1f;
    [SerializeField] SpriteRenderer detectIcon;
    [SerializeField] Transform healthBarPivot;
    [SerializeField] Transform healthBar;
    [SerializeField] ItemDrop[] loots;
    
    Vector3 detectIconBaseScale;
    [HideInInspector] public Vector2 startPosition;
    float alpha;
    bool pushed;
    SpriteRenderer[] healthBarSprites;

    public override void Awake()
    {
        base.Awake();
        rb.isKinematic = true;
        detectIconBaseScale = detectIcon.transform.localScale;
        detectIcon.gameObject.SetActive(false);
        healthBarSprites = healthBar.GetComponentsInChildren<SpriteRenderer>();
        startPosition = aiAgent.transform.position;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(startPosition, transform.position);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!pushed)
            return;

        if (collision.CompareTag("ObstacleTilemap"))
        {
            Stop();
            TakeDamages(5f);
        }
        else if (collision.CompareTag("GroundTilemap"))
            Death();
    }

    public override void TakeDamages(float amount, Entity aggressor = null, Vector2 impactPoint = default)
    {
        base.TakeDamages(amount, aggressor);
        healthBarPivot.DOScaleX((float)Health.CurrentValue / (float)Health.MaxValue, 0.3f);
    }

    public override void ManageAnimations()
    {
        base.ManageAnimations();
        if (aiAgent)
            anim.SetFloat("Speed", aiAgent.velocity.sqrMagnitude);
    }

    public override void Stun()
    {
        base.Stun();
        if (myBehavior)
            myBehavior.Stun();

        //pushed = true;
    }

    public override void Death()
    {
        base.Death();
        VFXManager.Instance.PlayVFX("BodyExplode", spriteRenderer.transform.position);
    }

    public override void UnStun()
    {
        base.UnStun();
        pushed = false;
        rb.isKinematic = true;
    }

    public void LaunchAttack()
    {
        anim.SetTrigger("Attack");
    }

    public override void ReactToTarget()
    {
        base.ReactToTarget();
        detectIcon.gameObject.SetActive(true);
        detectIcon.transform.DOKill();
        detectIcon.DOKill();

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

    public bool IsMoving()
    {
        return rb.velocity.sqrMagnitude > 1.5f;
    }

    void Flip(float otherX)
    {
        float distance = Vector2.Distance(transform.position, destinationPoint.position);
        if (distance < aiAgent.endReachedDistance)
            return;

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
    }
}
