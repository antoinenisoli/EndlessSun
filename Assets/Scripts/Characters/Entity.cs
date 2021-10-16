using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Entity : MonoBehaviour
{
    [Header("Entity")]
    public Health health;
    [SerializeField] protected float baseSpeed = 5f;
    protected Rigidbody2D rb;
    protected Vector3 m_Velocity;
    public SpriteRenderer spr;
    protected Animator anim;
    protected bool stunned;

    public virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spr = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
    }

    public virtual void Start()
    {
        health.Initialize();
    }

    public virtual void ManageAnimations()
    {
        anim.SetFloat("Speed", rb.velocity.sqrMagnitude);
        anim.SetBool("Dead", health.isDead);
    }

    public virtual void Death()
    {
        anim.SetTrigger("Death");
    }

    public virtual void Hit(float amount, Vector2 force = new Vector2())
    {
        if (health.isDead)
            return;

        health.ModifyValue(-amount);
        anim.SetTrigger("Hit");
        spr.transform.DOPunchScale(Vector3.one * 0.5f, 0.1f);
        rb.AddForce(force, ForceMode2D.Impulse);
        if (health.isDead)
            Death();
    }

    public virtual void Update()
    {
        ManageAnimations();
    }
}
