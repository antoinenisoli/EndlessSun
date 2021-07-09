using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float movementSmoothing = 0.05f;

    [SerializeField] LayerMask enemyLayer;
    [SerializeField] float attackDelay = 0.1f;
    float attackTimer;
    [SerializeField] int attackAnimCount = 3;
    [SerializeField] Transform attackPoint;
    [SerializeField] float attackRadius = 1;

    Rigidbody2D rb;
    Vector3 m_Velocity;
    SpriteRenderer spr;
    Animator anim;

    float inputX, inputY;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spr = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
    }

    void Inputs()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");

        if (inputX < 0 && spr.gameObject.transform.rotation.eulerAngles == Vector3.zero)
            spr.gameObject.transform.Rotate(Vector3.up * 180);

        if (inputX > 0 && spr.gameObject.transform.rotation.eulerAngles == Vector3.up * 180)
            spr.gameObject.transform.Rotate(Vector3.up * -180);
    }

    void ManageAnimations()
    {
        anim.SetBool("Run", rb.velocity.sqrMagnitude > 0.1f);
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
            Attack();
    }

    void Attack()
    {
        attackTimer = 0;
        anim.SetTrigger("Attack" + Random.Range(0, attackAnimCount).ToString());
        Collider2D[] colls = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, enemyLayer);
        foreach (var item in colls)
        {
            print(item);
            Enemy enemy = item.transform.GetComponent<Enemy>();
            if (enemy)
                enemy.Hit();
        }
    }

    private void Update()
    {
        Inputs();
        Move();
        ManageAnimations();
        ManageAttacks();
    }
}
