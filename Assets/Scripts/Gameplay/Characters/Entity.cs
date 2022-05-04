using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public enum Team
{
    Player,
    NPCs,
    Monsters,
    Natives,
    Animals,
}

public class Entity : MonoBehaviour
{
    public virtual HealthStat Health => health;
    public AttributeList AttributeList => CharacterProfile.AttributeList;

    [Header("Entity")]
    public RelationManager relationManager;
    [SerializeField] protected Material hitMat;
    [SerializeField] protected float pushForce = 0.5f;
    protected Material baseMat;
    public Entity Target;
    public LayerMask targetLayer;
    public SpriteRenderer spr;
    [SerializeField] protected CharacterProfile profileToCopy;
    [HideInInspector] public CharacterProfile CharacterProfile;
    [SerializeField] protected HealthStat health;

    protected Rigidbody2D rb;
    protected Vector3 m_Velocity;
    protected Animator anim;
    protected Entity oldTarget;
    protected List<CharacterMod> myMods = new List<CharacterMod>();

    public virtual void Awake()
    {    
        if (profileToCopy)
            CharacterProfile = Instantiate(profileToCopy);

        relationManager.Init();
        rb = GetComponentInParent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        InitStats();
        baseMat = spr.material;
    }

    public virtual void Start() { }

    void InitStats()
    {
        AttributeList.Init();

        Health.MaxValue = CharacterProfile.Health.MaxValue;
        Health.BaseMaxValue = CharacterProfile.Health.BaseMaxValue;
        Health.Init(this);
    }

    public virtual void SetTarget(Entity target)
    {
        if (Target != null)
            oldTarget = Target;

        Target = target;
    }

    public void AddMod(CharacterMod mod)
    {
        myMods.Add(mod);
    }

    public virtual float BaseSpeed() { return AttributeList.Speed.value; }

    public int ComputeDamages()
    {
        return AttributeList.ComputeDamages();
    }

    public virtual void Stop() 
    { 
        //rb.velocity = new Vector2(); 
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
        return AttributeList.BalanceDraw(target);
    }

    public void Push(Vector2 force)
    {
        Stun();
        rb.AddForce(force, ForceMode2D.Impulse);
    }

    public virtual void TakeDamages(float amount, Entity aggressor = null, Vector2 impactPoint = default)
    {
        Health.ModifyValue(-amount);
        StartCoroutine(Flash());
        if (Health.isDead)
            Death();
    }

    #region "ToOverride"
    public virtual void Attack() { }
    public virtual void Stun() { }
    public virtual void UnStun() { }
    #endregion

    #region "Relations"
    public bool SameTeam(Entity other) => relationManager.myTeam == other.relationManager.myTeam;
    public bool IsHostile(Entity other) => !SameTeam(other) && relationManager.CheckRelation(RelationType.Hostile, other);
    public bool IsFriendly(Entity other) => SameTeam(other) || relationManager.CheckRelation(RelationType.Friendly, other);
    public bool IsNeutral(Entity other) => !SameTeam(other) && relationManager.CheckRelation(RelationType.Neutral, other);
    #endregion

    IEnumerator Flash()
    {
        spr.material = hitMat;
        spr.transform.DOComplete();
        spr.transform.DOPunchScale(Vector3.one * -0.2f, 0.1f);
        yield return new WaitForEndOfFrame();
        spr.material = baseMat;
    }

    public virtual void DoUpdate()
    {
        ManageAnimations();
        if (Target && Target.health.isDead)
            SetTarget(null);
    }

    public void Update()
    {
        DoUpdate();
    }
}
