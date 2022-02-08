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
    Deactivated,
}

public class PlayerController2D : Entity
{
    public PlayerSurvival Survival => PlayerSurvival.Instance;
    public override HealthStat Health => playerHealth;

    [Header("PLAYER")]
    [SerializeField] LayerMask tilemapLayer;
    public PlayerHealth playerHealth;
    [Space(10)]
    public PlayerXP myXP;
    [Space(10)]
    public PlayerCombat Combat;
    [Space(10)]
    public PlayerMagic Magic;
    [Space(10)]
    PlayerMod[] mods;

    [Header("__Sprint")]
    public PlayerState currentState;
    [SerializeField] float sprintCost = 15f;
    [SerializeField] float movementSmoothing = 0.05f;
    bool sprinting;

    [Header("__Attack")]
    [SerializeField] int attackAnimCount = 3;
    public bool idleSword;

    [Header("__Items")]
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
        mods = new PlayerMod[] 
        { 
            Combat, 
            Magic,
        };

        foreach (var item in mods)
            item.Init();
    }

    public override void Start()
    {
        base.Start();
        myXP.GenerateLevels();
    }

    public void CheckCollision()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.5f, tilemapLayer))
        {
            Region r = GridManager.Instance.BiggestIsland();
            if (r != null)
            {
                Vector2 newPos = r.ClosestGroundPos(r.CenterPosition());
                transform.position = newPos;
            }
        }
    }

    bool CanMove()
    {
        return 
            currentState != PlayerState.Attacking 
            && currentState != PlayerState.Dead
            && currentState != PlayerState.Deactivated
            ;
    }

    void Inputs()
    {
        if (CanMove())
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

    public override void ManageAnimations()
    {
        base.ManageAnimations();
        if (idleSword && rb.velocity.sqrMagnitude > 0.1f)
            idleSword = false;

        anim.SetBool("IdleSword", idleSword);
        anim.SetFloat("Speed", rb.velocity.sqrMagnitude);
        anim.speed = Mathf.Lerp(anim.speed, sprinting ? 2 : 1, Time.deltaTime * 10f);
    }

    public override void Death()
    {
        base.Death();
        rb.velocity = new Vector2();
        SetState(PlayerState.Dead);
        anim.SetTrigger("Die");
    }

    public override void Hit(float amount, Entity aggressor = null)
    {
        base.Hit(amount, aggressor);
        GameManager.Instance.FreezeFrame(0.4f);
        StartCoroutine(Glow(0.1f, Color.white));
        if (CameraManager.Instance) CameraManager.Instance.CameraShake(0.3f, 2);
        if (Health.isDead)
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

    public override float ComputeSpeed()
    {
        return sprinting ? runSpeed : walkSpeed; 
    }

    void Move()
    {
        Vector2 inputs = new Vector2(inputX, inputY);
        sprinting = Input.GetKey(KeyCode.LeftShift) && inputs.sqrMagnitude > 0.1f && PlayerCombat.Stamina.CurrentValue > 0;
        if (sprinting)
            PlayerCombat.Stamina.StaminaCost(sprintCost);

        Vector2 targetVelocity = inputs.normalized * ComputeSpeed();
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref m_Velocity, movementSmoothing);
    }

    void ManageAttacks()
    {
        if (CanMove() && !EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetButtonDown("MainAttack"))
                LaunchAttack(true);
            else if (Input.GetButtonDown("SecondaryAttack"))
                LaunchAttack(false);
        }
    }

    void LaunchAttack(bool sword)
    {
        if (Combat.EnoughStamina())
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

    public override void Update()
    {
        base.Update();
        Inputs();
        Move();
        ManageAttacks();
        ManageStates();
        Survival.Update();
        playerHealth.Update();
        foreach (var item in mods)
            item.Update();

        if (CanMove())
        {
            if (lastDetectedInteractable)
            {
                if (Input.GetButtonDown("Interaction"))
                    lastDetectedInteractable.Interact();
            }

            if (Input.GetButtonDown("Inventory"))
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
