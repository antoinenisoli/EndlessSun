using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlayerArrow : MonoBehaviour
{
    [SerializeField] Transform arrowSprite;
    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Enemy enemy))
        {
            enemy.Hit(1, rb.velocity * 0.5f);
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (rb.velocity.sqrMagnitude < 0.1f)
            Destroy(gameObject);
    }
}
