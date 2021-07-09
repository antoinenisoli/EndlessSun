using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Rigidbody2D rb;
    Vector3 m_Velocity;
    SpriteRenderer spr;
    Animator anim;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spr = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
    }

    public void Hit()
    {
        anim.SetTrigger("Death");
    }
}
