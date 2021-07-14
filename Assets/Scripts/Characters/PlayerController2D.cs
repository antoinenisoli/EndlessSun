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
    public PlayerState currentState;
    [SerializeField] float speed;
    [SerializeField] float movementSmoothing = 0.05f;

    [Header("Attack")]
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] float attackDelay = 0.1f;
    float attackTimer;
    [SerializeField] int attackAnimCount = 3;
    [SerializeField] [Range(0,1)] float attackRadius = 0.5f, attackRange = 0.5f;
    [SerializeField] float pushForce = 5f;
    bool idleSword;

    float inputX, inputY;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, spr.transform.right * attackRange);
        Gizmos.DrawWireSphere(transform.position + spr.transform.right * attackRange, attackRadius);
        Color clone = Color.red;
        clone.a = 0.25f;
        Gizmos.color = clone;
        Gizmos.DrawSphere(transform.position + spr.transform.right * attackRange, attackRadius);
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

        anim.SetBool("Run", rb.velocity.sqrMagnitude > 0.1f);
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
        if (attackTimer <= attackDelay)
            attackTimer += Time.deltaTime;

        if (Input.GetMouseButtonDown(0) && attackTimer >= attackDelay)
            LaunchAttack();
    }

    void LaunchAttack()
    {
        attackTimer = 0;
        rb.velocity = new Vector3();
        SetState(PlayerState.Attacking);
        anim.SetTrigger("Attack" + Random.Range(0, attackAnimCount).ToString());

        float point = Input.mousePosition.x;
        if (point < Screen.width/2)
            spr.transform.eulerAngles = new Vector3(0, 180, 0);
        else
            spr.transform.eulerAngles = new Vector3(0, 0, 0);
    }

    public void Attack()
    {
        RaycastHit2D[] colls = Physics2D.CircleCastAll(transform.position, attackRadius, spr.transform.right, attackRange, enemyLayer);
        idleSword = true;
        if (colls.Length > 0)
            GameManager.Instance.CameraShake(0.2f);
        else
            return;

        foreach (var item in colls)
        {
            Enemy enemy = item.transform.GetComponent<Enemy>();
            if (enemy)
                enemy.Hit(-item.normal * pushForce);
        }
    }

    void ManageStates()
    {
        if (attackTimer >= attackDelay && currentState != PlayerState.Idle)
            SetState(PlayerState.Idle);
        if (rb.velocity.sqrMagnitude > 0.1f && currentState != PlayerState.Moving)
            SetState(PlayerState.Moving);
    }

    private void Update()
    {
        Inputs();
        Move();
        ManageAnimations();
        ManageAttacks();
        ManageStates();
    }
}
