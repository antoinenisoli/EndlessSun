using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    Idle,
    Moving,
    Attacking,
}

public class PlayerController2D : Entity
{
    [Header("PLAYER")]
    public PlayerSurvival Survival;
    public PlayerXP myXP;
    public PlayerCombat Combat;
    public PlayerState currentState;
    [SerializeField] float speed;
    [SerializeField] float movementSmoothing = 0.05f;

    [Header("Attack")]
    [SerializeField] float staminaCost = 15f;
    [SerializeField] int attackAnimCount = 3;
    public bool idleSword;

    float inputX, inputY;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, spr.transform.right * Combat.attackRange);
        Gizmos.DrawWireSphere(transform.position + spr.transform.right * Combat.attackRange, Combat.attackRadius);
        Color clone = Color.red;
        clone.a = 0.25f;
        Gizmos.color = clone;
        Gizmos.DrawSphere(transform.position + spr.transform.right * Combat.attackRange, Combat.attackRadius);
    }

    public override void Awake()
    {
        base.Awake();
        Survival.Init();
        Combat.Init();
    }

    public override void Start()
    {
        base.Start();
        myXP.GenerateLevels();
    }

    [ContextMenu("Write names")]
    public override void WriteName()
    {
        base.WriteName();
        foreach (var item in Survival.stats)
            item.statName = item.thisStat.ToString();

        foreach (var item in Combat.stats)
            item.statName = item.thisStat.ToString();
    }

    void Inputs()
    {
        if (currentState != PlayerState.Attacking)
        {
            inputX = Input.GetAxisRaw("Horizontal");
            inputY = Input.GetAxisRaw("Vertical");

            if (inputX < 0 && spr.transform.rotation.eulerAngles == Vector3.zero)
                spr.transform.eulerAngles = new Vector3(0, 180, 0);
            if (inputX > 0 && spr.transform.rotation.eulerAngles == Vector3.up * 180)
                spr.transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else
        {
            inputX = 0;
            inputY = 0;
        }
    }

    public void SetState(PlayerState newState)
    {
        currentState = newState;
    }

    void ManageAnimations()
    {
        if (idleSword && rb.velocity.sqrMagnitude > 0.1f)
            idleSword = false;

        anim.SetFloat("Speed", rb.velocity.sqrMagnitude);
        anim.SetBool("IdleSword", idleSword);
    }

    void Move()
    {
        Vector2 inputs = new Vector2(inputX, inputY);
        Vector2 targetVelocity = inputs.normalized * speed;
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref m_Velocity, movementSmoothing);
    }

    void ManageAttacks()
    {
        if (currentState != PlayerState.Attacking)
        {
            if (Input.GetMouseButtonDown(0))
                LaunchAttack(true);
            else if (Input.GetMouseButtonDown(1))
                LaunchAttack(false);
        }
    }

    void LaunchAttack(bool sword)
    {
        if (PlayerCombat.Stamina.CurrentValue > staminaCost)
        {
            rb.velocity = new Vector3();
            SetState(PlayerState.Attacking);
            if (!sword)
            {
                anim.SetTrigger("Bow");
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 vel = mousePos - (Vector2)transform.position;
                vel.Normalize();
                Combat.storedVelocity = vel;
            }
            else
                anim.SetTrigger("Attack" + Random.Range(0, attackAnimCount).ToString());

            float point = Input.mousePosition.x;
            if (point < Screen.width / 2)
                spr.transform.eulerAngles = new Vector3(0, 180, 0);
            else
                spr.transform.eulerAngles = new Vector3(0, 0, 0);
        }
    }

    void ManageStates()
    {
        if (rb.velocity.sqrMagnitude < 0.1f && currentState == PlayerState.Moving)
            SetState(PlayerState.Idle);
        if (rb.velocity.sqrMagnitude > 0.1f && currentState == PlayerState.Idle)
            SetState(PlayerState.Moving);
    }

    private void Update()
    {
        Inputs();
        Move();
        ManageAnimations();
        ManageAttacks();
        ManageStates();
        Survival.Update();
        Combat.Update();
    }
}
