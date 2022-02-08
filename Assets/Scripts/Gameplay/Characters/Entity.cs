using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum Team
{
    Player,
    Enemy,
}

public class Entity : MonoBehaviour
{
    public virtual HealthStat Health => health;
    public AttributeList AttributeList { get => attributeList; set => attributeList = value; }
    public Entity Target { get; set; }

    [Header("Entity")]
    public Team myTeam;
    public LayerMask targetLayer;
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
    protected List<Entity> aggressors = new List<Entity>();

    public virtual void Awake()
    {
        rb = GetComponentInParent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        if (profile)
            AttributeList = profile.AttributeList.Copy();

        AttributeList.Init();
        Health.Init();
    }

    public virtual void Start()
    {
        
    }

    public virtual void NewAgressor(Entity agressor) 
    {
        aggressors.Add(agressor);
    }

    public virtual float ComputeSpeed() { return walkSpeed; }

    public void SetTarget(Entity target)
    {
        Target = target;
    }

    public int ComputeDamages()
    {
        return AttributeList.ComputeDamages();
    }

    public virtual void ManageAnimations()
    {
        anim.SetBool("Dead", Health.isDead);
    }

    public virtual void Death()
    {
        health.CurrentValue = 0;
        anim.SetTrigger("Death");
    }

    public bool BalanceDraw(Entity target)
    {
        return attributeList.BalanceDraw(target);
    }

    public void KnockBack(Vector2 force)
    {
        Stun();
        rb.AddForce(force, ForceMode2D.Impulse);
    }

    public virtual void Attack()
    {
        Target.Hit(ComputeDamages());
    }

    public virtual void Stun()
    {
        
    }

    public virtual void Hit(float amount, Entity aggressor = null)
    {
        if (Health.isDead)
            return;

        Health.ModifyValue(-amount);
        anim.SetTrigger("Hit");
        spr.transform.DOComplete();
        spr.transform.DOPunchScale(Vector3.one * -0.2f, 0.1f);
        NewAgressor(aggressor);

        if (Health.isDead)
            Death();
    }

    public virtual void Update()
    {
        ManageAnimations();
    }
}
