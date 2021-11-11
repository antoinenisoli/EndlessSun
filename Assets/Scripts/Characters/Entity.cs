using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Entity : MonoBehaviour
{
    public virtual HealthStat Health => health;
    public AttributeList AttributeList { get => attributeList; set => attributeList = value; }

    [Header("Entity")]
    public SpriteRenderer spr;
    [SerializeField] CharacterProfile profile;
    [SerializeField] AttributeList attributeList;
    [SerializeField] HealthStat health;

    [Header("_Movements")]
    [SerializeField] protected float walkSpeed = 5f;
    [SerializeField] protected float runSpeed = 10f;

    protected Rigidbody2D rb;
    protected Vector3 m_Velocity;
    protected Animator anim;
    protected bool stunned;

    public virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        if (profile)
            AttributeList = profile.AttributeList;

        AttributeList.Init();
    }

    public virtual void Start()
    {
        Health.Init();
    }

    public int ComputeDamages()
    {
        return AttributeList.ComputeDamages();
    }

    public virtual void ManageAnimations()
    {
        anim.SetFloat("Speed", rb.velocity.sqrMagnitude);
        anim.SetBool("Dead", Health.isDead);
    }

    public virtual void Death()
    {
        anim.SetTrigger("Death");
    }

    public bool BalanceDraw(Entity target)
    {
        print(attributeList.BalanceDraw(target));
        return attributeList.BalanceDraw(target);
    }

    public virtual void Hit(float amount, Vector2 force = new Vector2())
    {
        if (Health.isDead)
            return;

        Health.ModifyValue(-amount);
        anim.SetTrigger("Hit");
        spr.transform.DOPunchScale(Vector3.one * 0.5f, 0.1f);
        rb.AddForce(force, ForceMode2D.Impulse);
        if (Health.isDead)
            Death();
    }

    public virtual void Update()
    {
        ManageAnimations();
    }
}
