using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlayerArrow : MonoBehaviour
{
    PlayerController2D player => GameManager.Player;

    [SerializeField] Transform arrowSprite;
    [SerializeField] float minimumForce = 10f;
    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Entity entity = collision.GetComponentInChildren<Entity>();
        if (entity == player)
            return;

        if (entity && !entity.Health.isDead && rb.velocity.sqrMagnitude > minimumForce)
        {
            entity.TakeDamages(1, player, transform.position);
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (rb.velocity.sqrMagnitude < 0.1f)
            Destroy(gameObject);
    }
}
