using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public enum EntityState
{
    Normal,
    InFight,
    Wait,
}

public class Entity : MonoBehaviour
{
    public virtual HealthStat Health => health;
    public AttributeList AttributeList => CharacterProfile.AttributeList;

    [Header("Entity")]
    public EntityState entityState;
    public RelationManager relationManager;
    [SerializeField] protected Material hitMat;
    [SerializeField] protected float pushForce = 0.5f;
    protected Material baseMat;
    public Entity MainTarget;
    public LayerMask targetLayer;
    public SpriteRenderer spriteRenderer;
    [SerializeField] protected CharacterProfile profileToCopy;
    [SerializeField] protected HealthStat health;
    public List<Entity> engagedEntities = new List<Entity>();

    protected Rigidbody2D rb;
    protected Vector3 m_Velocity;
    protected Animator anim;
    protected List<CharacterMod> myMods = new List<CharacterMod>();
    [HideInInspector] public CharacterProfile CharacterProfile;

    public virtual void Awake()
    {    
        if (profileToCopy)
            CharacterProfile = Instantiate(profileToCopy);

        relationManager.Init();
        rb = GetComponentInParent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        InitStats();
        baseMat = spriteRenderer.material;
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
        if (!target)
            if (MainTarget && engagedEntities.Contains(MainTarget))
                engagedEntities.Remove(MainTarget);

        MainTarget = target;
    }

    public void NewTarget(Entity target)
    {
        if (!engagedEntities.Contains(target))
        {
            engagedEntities.Add(target);
            Stop();
            SetTarget(target);
        }
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

    public virtual void SetEntityState(EntityState newState)
    {
        entityState = newState;
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
        print("push!!");
        rb.AddForce(force, ForceMode2D.Impulse);
    }

    protected virtual void HitFeedback()
    {
        anim.SetTrigger("Hit");
        GameObject blood = VFXManager.Instance.PlayVFX("BloodFX", spriteRenderer.transform.position);
        int random = Random.Range(0, 2);
        blood.transform.Rotate(Vector2.up * 180 * random);
    }

    public virtual void TakeDamages(float amount, Entity aggressor = null, Vector2 impactPoint = default)
    {
        HitFeedback();
        rb.isKinematic = false;
        if (aggressor)
        {
            NewTarget(aggressor);
            if (aggressor.BalanceDraw(this))
            {
                Vector2 direction = (impactPoint - (Vector2)transform.position).normalized;
                Push(direction * pushForce);
            }
        }

        Health.ModifyValue(-amount);
        StopCoroutine(Flash());
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
        spriteRenderer.material = hitMat;
        spriteRenderer.transform.DOComplete();
        spriteRenderer.transform.DOPunchScale(Vector3.one * -0.2f, 0.1f);
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.material = baseMat;
    }

    void ManageEngagedEntities()
    {
        if (MainTarget && MainTarget.health.isDead)
            SetTarget(null);

        for (int i = 0; i < engagedEntities.Count; i++)
        {
            Entity thisEntity = engagedEntities[i];
            if (thisEntity.health.isDead)
                engagedEntities.Remove(thisEntity);
        }

        if (engagedEntities.Count > 0)
        {
            if (!MainTarget)
                MainTarget = engagedEntities[0];

            if (entityState == EntityState.Normal)
                SetEntityState(EntityState.InFight);
        }
        else
        {
            if (entityState == EntityState.InFight)
                SetEntityState(EntityState.Normal);
        }
    }

    public virtual void DoUpdate()
    {
        ManageAnimations();
        ManageEngagedEntities();
    }

    public void Update()
    {
        DoUpdate();
    }
}
