using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LootItem : Interactable
{
    [Header("Spawn animation")]
    [SerializeField] protected Transform itemTransform;
    [SerializeField] protected float animDuration = 1;
    [SerializeField] protected float spawnRadius = 0.5f;
    protected bool canBePicked;

    public virtual void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
        Gizmos.color = Color.blue;
    }

    public void Awake()
    {
        SpawnAnim();
    }

    public void SpawnAnim()
    {
        transform.DOKill();
        itemTransform.DOKill();
        Vector2 randomPos = Random.insideUnitCircle * spawnRadius;
        itemTransform.DOLocalJump(Vector2.zero, 1f, 1, animDuration);
        transform.DOMove((Vector2)transform.position + randomPos, animDuration).OnComplete(() => { canBePicked = true; });
    }
}
