using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public enum PlayerState
{
    Idle,
    Moving,
    Attacking,
    Dead,
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
        if (currentState != PlayerState.Attacking && currentState != PlayerState.Dead)
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
        anim.SetBool("Dead", health.isDead);
    }

    public override void Death()
    {
        base.Death();
        rb.velocity = new Vector2();
        SetState(PlayerState.Dead);
        anim.SetTrigger("Die");
    }

    IEnumerator FreezeFrame(float delay, float startScale = 0.1f)
    {
        float timer = 0;
        Time.timeScale = startScale;

        while (timer < delay)
        {
            yield return null;
            timer += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(Time.timeScale, 1, timer / delay);
        }

        Time.timeScale = 1f;
    }

    public void Hurt(float amount)
    {
        health.ModifyValue(-amount);
        StartCoroutine(FreezeFrame(0.4f));
        StartCoroutine(Glow(0.1f, Color.red));
        CameraManager.Instance.CameraShake(0.3f, 2);

        if (health.isDead)
            Death();
    }

    public void LevelUp()
    {
        CameraManager.Instance.UnZoom();
        UIManager.Instance.LevelUp();
        StopAllCoroutines();
        StartCoroutine(LevelUpFeedback(1.5f));
        StartCoroutine(Glow(1.5f, Color.white));
        StartCoroutine(FreezeFrame(1.5f));
    }

    IEnumerator Glow(float delay, Color targetColor = new Color())
    {
        Color col = new Color();
        float timer = 0;

        while (timer < delay)
        {
            yield return null;
            timer += Time.unscaledDeltaTime;
            col = Color.Lerp(col, targetColor, timer / delay);
            spr.material.SetColor("_GlobalGlow", col);
        }

        spr.material.SetColor("_GlobalGlow", Color.black);
    }

    IEnumerator LevelUpFeedback(float delay)
    {
        float timer = 0;
        VFXManager.Instance.PlayVFX("LevelUp", transform.position, true, transform);
        CameraManager.Instance.CameraShake(delay/2);

        while (timer < delay)
        {
            yield return null;
            rb.velocity = Vector2.zero;
            timer += Time.unscaledDeltaTime;
        }
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
            PlayerCombat.Stamina.StopRecovery();
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

        if (Input.GetKeyDown(KeyCode.E))
            Hurt(45);
    }
}
