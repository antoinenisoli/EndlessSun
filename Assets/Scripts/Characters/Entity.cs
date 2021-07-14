using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Entity : MonoBehaviour
{
    [Header("Entity")]
    public Health health;
    protected Rigidbody2D rb;
    protected Vector3 m_Velocity;
    [SerializeField] protected SpriteRenderer spr;
    protected Animator anim;

    public virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spr = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
        health.Initialize();
    }

    public virtual void Death()
    {
        anim.SetTrigger("Death");
    }

    public virtual void Hit(Vector2 force = new Vector2())
    {
        if (health.isDead)
            return;

        health.ModifyValue(-1);
        anim.SetTrigger("Hit");
        spr.transform.DOPunchScale(Vector3.one * 0.5f, 0.1f);
        rb.AddForce(force, ForceMode2D.Impulse);
        if (health.isDead)
            Death();
    }
}
