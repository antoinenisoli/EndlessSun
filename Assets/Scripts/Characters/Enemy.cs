using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    [Header("ENEMY")]
    [SerializeField] GameObject xpPrefab;
    [SerializeField] int xpAmount = 25;
    [SerializeField] ItemDrop[] loots;

    public override void Death()
    {
        base.Death();
        foreach (var item in loots)
        {
            for (int i = 0; i < item.dropCount; i++)
            {
                float random = Random.value;
                if (random < item.prob)
                    Instantiate(item.itemPrefab, transform.position, Quaternion.identity);
            }
        }

        int result = xpAmount / xpPrefab.GetComponent<XPItem>().xpAmount;
        for (int i = 0; i < result; i++)
            Instantiate(xpPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject, 1f);
    }
}
