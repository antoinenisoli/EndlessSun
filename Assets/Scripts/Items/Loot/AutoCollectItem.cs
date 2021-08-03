using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AutoCollectItem : LootItem
{
    [Header("AUTO COLLECT ITEM")]
    bool dragging;

    public override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        bool onPlayer = collision.gameObject == GameManager.Player.gameObject;
        if (!onPlayer)
            return;

        if (canBePicked)
        {
            Effect(GameManager.Player);
            Destroy(gameObject);
        }
    }

    void ManagePickUp()
    {
        if (canBePicked && !dragging)
        {
            float dist = Vector2.Distance(GameManager.Player.transform.position, transform.position);
            if (dist < pickupRadius)
                dragging = true;
        }

        if (dragging)
            transform.position = Vector3.MoveTowards(transform.position, GameManager.Player.transform.position, Time.deltaTime * 5f);
    }

    private void Update()
    {
        ManagePickUp();
    }

    public abstract void Effect(PlayerController2D player);
}
