using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public enum PlayerState
{
    Idle,
    InFight,
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

    [Header(nameof(PlayerController2D))]
    public PlayerState playerState;
    [SerializeField] LayerMask tilemapLayer;

    [Header("__Sprint")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    [SerializeField] float sprintCost = 15f;
    [SerializeField] float movementSmoothing = 0.05f;
    bool sprinting;

    [Header("__Attack")]
    [SerializeField] int attackAnimCount = 3;

    [Header("__Items")]
    [SerializeField] float interactionRadius = 2f;
    [SerializeField] LayerMask interactLayer;
    public Interactable lastDetectedInteractable;

    #region "Mods"
    public PlayerHealth playerHealth;
    public static CharacterManager xpManager;
    public static PlayerCombat Combat;
    public static PlayerMagic Magic;
    #endregion

    float inputX, inputY;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
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
            playerState != PlayerState.Attacking 
            && playerState != PlayerState.Dead
            && playerState != PlayerState.Deactivated
            ;
    }

    void Inputs()
    {
        if (CanMove())
        {
            inputX = Input.GetAxisRaw("Horizontal");
            inputY = Input.GetAxisRaw("Vertical");

            if (inputX < 0 && spriteRenderer.transform.rotation.eulerAngles == Vector3.zero)
                spriteRenderer.transform.eulerAngles = new Vector3(0, 180, 0);
            if (inputX > 0 && spriteRenderer.transform.rotation.eulerAngles == Vector3.up * 180)
                spriteRenderer.transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else
        {
            inputX = 0;
            inputY = 0;
        }
    }

    public void SetPlayerState(PlayerState newState)
    {
        playerState = newState;
    }

    public override void SetEntityState(EntityState newState)
    {
        if (newState != EntityState.Wait)
        {
            anim.SetTrigger("SetFightMode");
        }

        anim.SetFloat("StateIndex", (int)newState);
        base.SetEntityState(newState);
    }

    public override void ManageAnimations()
    {
        base.ManageAnimations();
        anim.SetFloat("Speed", rb.velocity.sqrMagnitude);
        anim.speed = Mathf.Lerp(anim.speed, sprinting ? 2 : 1, Time.deltaTime * 10f);
    }

    public override void Death()
    {
        base.Death();
        rb.velocity = new Vector2();
        SetPlayerState(PlayerState.Dead);
        anim.SetTrigger("Die");
    }

    protected override void HitFeedback()
    {
        base.HitFeedback();
        //Feedbacks.FreezeGame(0, 0.15f);
        Feedbacks.SlowMotion(0, 0.4f);
        if (CameraManager.Instance)
            CameraManager.Instance.CameraShake(0.25f, 15f);
    }

    public void LevelUp()
    {
        CameraManager.Instance.UnZoom();
        UIManager.Instance.LevelUp();
        StopAllCoroutines();
        StartCoroutine(LevelUpFeedback(1.5f));
        StartCoroutine(Glow(1.5f, Color.white));
        Feedbacks.SlowMotion(0, 0.4f);
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
            spriteRenderer.material.SetColor("_Tint", col);
        }

        spriteRenderer.material.SetColor("_Tint", Color.white);
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

    public override float BaseSpeed()
    {
        return sprinting ? runSpeed : walkSpeed; 
    }

    void Move()
    {
        Vector2 inputs = new Vector2(inputX, inputY);
        sprinting = Input.GetKey(KeyCode.LeftShift) && inputs.sqrMagnitude > 0.1f && PlayerCombat.Stamina.CurrentValue > 0;
        if (sprinting)
            PlayerCombat.Stamina.StaminaCost(sprintCost);

        Vector2 targetVelocity = inputs.normalized * BaseSpeed();
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref m_Velocity, movementSmoothing);
    }

    void ManageAttacks()
    {
        if (CanMove() && !EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetButtonDown("MainAttack"))
                LaunchPrimaryAttack();
            else if (Input.GetButtonDown("SecondaryAttack"))
                LaunchSecondaryAttack();
        }
    }

    void LaunchPrimaryAttack()
    {
        if (!Combat.EnoughStamina())
            return;

        LaunchAttack();
        anim.SetTrigger("MainAttack");
        anim.SetFloat("attackIndex", Random.Range(0, attackAnimCount));
    }

    public override void Stop()
    {
        base.Stop();
        rb.velocity = new Vector2();
    }

    void LaunchAttack()
    {
        PlayerCombat.Stamina.StopRecovery();
        Stop();
        SetPlayerState(PlayerState.Attacking);
        float point = Input.mousePosition.x;
        if (point < Screen.width / 2)
            spriteRenderer.transform.eulerAngles = new Vector3(0, 180, 0);
        else
            spriteRenderer.transform.eulerAngles = new Vector3(0, 0, 0);
    }

    void LaunchSecondaryAttack()
    {
        if (!Combat.EnoughStamina())
            return;

        LaunchAttack();
        anim.SetTrigger("SecondaryAttack");
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 vel = mousePos - (Vector2)transform.position;
        vel.Normalize();
        Combat.storedVelocity = vel;
    }

    void ManageStates()
    {
        if (playerState == PlayerState.Idle && entityState == EntityState.InFight)
            SetPlayerState(PlayerState.InFight);

        if (rb.velocity.sqrMagnitude < 0.1f && playerState == PlayerState.Moving)
        {
            if (entityState == EntityState.InFight)
                SetPlayerState(PlayerState.InFight);
            else
                SetPlayerState(PlayerState.Idle);
        }

        if (rb.velocity.sqrMagnitude > 0.1f && playerState != PlayerState.Moving)
            SetPlayerState(PlayerState.Moving);
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

    public override void DoUpdate()
    {
        base.DoUpdate();
        Inputs();
        Move();
        ManageAttacks();
        ManageStates();
        Survival.Update();
        playerHealth.Update();
        foreach (var item in myMods)
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
