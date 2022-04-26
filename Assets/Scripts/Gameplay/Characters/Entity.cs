using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public enum Team
{
    Player,
    Enemy,
    Neutral,
}

public class Entity : MonoBehaviour
{
    public virtual HealthStat Health => health;
    public AttributeList AttributeList => CharacterProfile.AttributeList;
    public Entity Target { get; set; }

    [Header("Entity")]
    public Team myTeam;
    public LayerMask targetLayer;
    public SpriteRenderer spr;
    [SerializeField] CharacterProfile profileToCopy;
    [HideInInspector] public CharacterProfile CharacterProfile;
    [SerializeField] HealthStat health;

    [Header("_Movements")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;

    protected Rigidbody2D rb;
    protected Vector3 m_Velocity;
    protected Animator anim;
    protected List<CharacterMod> myMods = new List<CharacterMod>();

    public virtual void Awake()
    {    
        if (profileToCopy)
            CharacterProfile = Instantiate(profileToCopy);

        rb = GetComponentInParent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        InitStats();
    }

    public virtual void Start() { }

    void InitStats()
    {
        AttributeList.Init();

        Health.MaxValue = CharacterProfile.Health.MaxValue;
        Health.BaseMaxValue = CharacterProfile.Health.BaseMaxValue;
        Health.Init(this);
    }

    public void AddMod(CharacterMod mod)
    {
        myMods.Add(mod);
    }

    public bool IsEnemyOf(NPC target)
    {
        return myTeam != target.myTeam;
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

    public virtual void Stop() { }

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
        return AttributeList.BalanceDraw(target);
    }

    public void KnockBack(Vector2 force)
    {
        Stun();
        rb.AddForce(force, ForceMode2D.Impulse);
    }

    public bool SameTeam(Entity other)
    {
        return myTeam == other.myTeam;
    }

    public virtual void Attack()
    {
        Target.Hit(ComputeDamages());
    }

    public virtual void Stun() { }

    public virtual void UnStun() { }

    public virtual void Hit(float amount, Entity aggressor = null)
    {
        if (Health.isDead)
            return;

        Health.ModifyValue(-amount);
        //anim.SetTrigger("Hit");
        spr.transform.DOComplete();
        spr.transform.DOPunchScale(Vector3.one * -0.2f, 0.1f);

        if (Health.isDead)
            Death();
    }

    public virtual void DoUpdate()
    {
        ManageAnimations();
    }

    public void Update()
    {
        DoUpdate();
    }
}
