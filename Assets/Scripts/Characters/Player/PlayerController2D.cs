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
    public PlayerXP myXP;
    public PlayerState currentState;
    [SerializeField] float speed;
    [SerializeField] float movementSmoothing = 0.05f;

    [Header("Attack")]
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] int attackAnimCount = 3;
    [SerializeField] [Range(0,1)] float attackRadius = 0.5f, attackRange = 0.5f;
    [SerializeField] float pushForce = 5f;
    bool idleSword;

    [Header("Bow")]
    [SerializeField] GameObject arrowPrefab;
    [SerializeField] float arrowForce = 2f;
    Vector2 storedVelocity;

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

    private void Start()
    {
        myXP.GenerateLevels();
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
        rb.velocity = new Vector3();
        SetState(PlayerState.Attacking);
        if (!sword)
        {
            anim.SetTrigger("Bow");
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            storedVelocity = mousePos - (Vector2)transform.position;
            storedVelocity.Normalize();
        }
        else
            anim.SetTrigger("Attack" + Random.Range(0, attackAnimCount).ToString());

        float point = Input.mousePosition.x;
        if (point < Screen.width/2)
            spr.transform.eulerAngles = new Vector3(0, 180, 0);
        else
            spr.transform.eulerAngles = new Vector3(0, 0, 0);
    }

    public void Attack()
    {
        SetState(PlayerState.Idle);
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

    public void FireArrow()
    {
        SetState(PlayerState.Idle);
        GameObject arrow = Instantiate(arrowPrefab, transform.position, arrowPrefab.transform.rotation);
        Rigidbody2D arrowRB = arrow.GetComponent<Rigidbody2D>();
        float rot_z = Mathf.Atan2(storedVelocity.y, storedVelocity.x) * Mathf.Rad2Deg;
        arrow.transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
        arrowRB.AddForce(storedVelocity * arrowForce, ForceMode2D.Impulse);
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
    }
}
