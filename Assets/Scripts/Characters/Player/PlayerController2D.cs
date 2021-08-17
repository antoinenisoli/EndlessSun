using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public enum PlayerState
{
    Idle,
    Moving,
    Sprinting,
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
    [SerializeField] float sprintCost = 15f;
    [SerializeField] float baseSpeed = 4, sprintSpeed = 7;
    [SerializeField] float movementSmoothing = 0.05f;
    bool sprinting;

    [Header("Attack")]
    [SerializeField] float staminaCost = 15f;
    [SerializeField] int attackAnimCount = 3;
    public bool idleSword;

    [Header("Items")]
    [SerializeField] float interactionRadius = 2f;
    [SerializeField] LayerMask interactLayer;
    public Interactable lastDetectedInteractable;

    float inputX, inputY;

    private void OnDrawGizmos()
    {
        Combat.Gizmo(this);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
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
        anim.speed = Mathf.Lerp(anim.speed, sprinting ? 2 : 1, Time.deltaTime * 10f);
    }

    public override void Death()
    {
        base.Death();
        rb.velocity = new Vector2();
        SetState(PlayerState.Dead);
        anim.SetTrigger("Die");
    }

    public void Hurt(float amount)
    {
        health.ModifyValue(-amount);
        GameManager.Instance.FreezeFrame(0.4f);
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
        GameManager.Instance.FreezeFrame(0.4f);
    }

    public IEnumerator Glow(float delay, Color targetColor = new Color())
    {
        float timer = 0;
        float intensity = 0;

        while (timer < delay)
        {
            yield return null;
            timer += Time.unscaledDeltaTime;
            intensity = Mathf.Lerp(intensity, 2f, timer / delay);
            float factor = Mathf.Pow(2, intensity);
            Color col = new Color(targetColor.r * factor, targetColor.g * factor, targetColor.b * factor);
            spr.material.SetColor("_Tint", col);
        }

        spr.material.SetColor("_Tint", Color.white);
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
        sprinting = Input.GetKey(KeyCode.LeftShift) && inputs.sqrMagnitude > 0.1f && PlayerCombat.Stamina.CurrentValue > 0;
        if (sprinting)
            PlayerCombat.Stamina.StaminaCost(sprintCost);

        Vector2 targetVelocity = inputs.normalized * (sprinting ? sprintSpeed : baseSpeed);
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref m_Velocity, movementSmoothing);
    }

    void ManageAttacks()
    {
        if (currentState != PlayerState.Attacking && !EventSystem.current.IsPointerOverGameObject())
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

    Interactable ClosestInteractable()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, interactionRadius, interactLayer);
        float inf = Mathf.Infinity;
        Interactable currentInteractable = null;

        foreach (var collider in cols)
        {
            float distance = Vector3.Distance(transform.position, collider.transform.position);
            if (distance < inf)
            {
                Interactable item = collider.GetComponent<Interactable>();
                inf = distance;
                currentInteractable = item;
            }
        }

        return currentInteractable;
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

        if (lastDetectedInteractable)
        {
            if (Input.GetKeyDown(KeyCode.E))
                lastDetectedInteractable.Interact();
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            UIManager.Instance.inventoryUI.Switch();
        }
    }

    void LateUpdate()
    {
        if (lastDetectedInteractable)
            lastDetectedInteractable.ProposeToInteract();
        else
            UIManager.Instance.ShowPickUp(null);
    }

    private void FixedUpdate()
    {
        lastDetectedInteractable = ClosestInteractable();
    }
}
