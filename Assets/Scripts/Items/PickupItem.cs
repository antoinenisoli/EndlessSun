using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public abstract class PickupItem : MonoBehaviour
{
    [SerializeField] protected Transform item;
    [SerializeField] protected float animDuration = 1;
    [SerializeField] protected float spawnRadius = 0.5f;
    [SerializeField] protected float pickupRadius = 1f;
    protected bool canBePicked;
    bool dragging;

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (canBePicked && collision.gameObject == GameManager.Player.gameObject)
        {
            Effect(GameManager.Player);
            Destroy(gameObject);
        }
    }

    private void Awake()
    {
        SpawnAnim();
    }

    public abstract void Effect(PlayerController2D player);

    void SpawnAnim()
    {
        transform.DOKill();
        item.DOKill();
        Vector2 randomPos = Random.insideUnitCircle * spawnRadius;
        item.DOLocalJump(Vector2.zero, 1f, 1, animDuration);
        transform.DOMove((Vector2)transform.position + randomPos, animDuration).OnComplete(() => { canBePicked = true; });
    }

    private void Update()
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
}
