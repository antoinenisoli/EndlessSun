using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PickupItem : MonoBehaviour
{
    [SerializeField] Transform item;
    [SerializeField] float animDuration = 1;
    [SerializeField] float spawnRadius = 0.5f;
    [SerializeField] float pickupRadius = 1f;
    bool canBePicked;

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }

    private void Awake()
    {
        SpawnAnim();
    }

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
        if (Input.GetKeyDown(KeyCode.E))
            SpawnAnim();

        if (canBePicked)
        {
            float dist = Vector2.Distance(GameManager.Player.transform.position, transform.position);
            if (dist < pickupRadius)
            {
                transform.position = Vector3.MoveTowards(transform.position, GameManager.Player.transform.position, Time.deltaTime * 5f);
            }
        }
    }
}
