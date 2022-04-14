using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlayerArrow : MonoBehaviour
{
    [SerializeField] Transform arrowSprite;
    [SerializeField] float minimumForce = 10f;
    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        NPC npc = collision.GetComponentInChildren<NPC>();
        if (npc && GameManager.Player.IsEnemyOf(npc) && rb.velocity.sqrMagnitude > minimumForce)
        {
            npc.Hit(1, GameManager.Player);
            if (GameManager.Player.BalanceDraw(npc))
                npc.KnockBack(rb.velocity * 0.5f);

            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (rb.velocity.sqrMagnitude < 0.1f)
            Destroy(gameObject);
    }
}
