using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    [SerializeField] ItemDrop[] loots;

    public override void Death()
    {
        base.Death();
        foreach (var item in loots)
        {
            float random = Random.value;
            if (random < item.prob)
                Instantiate(item.itemPrefab, transform.position, Quaternion.identity);
        }
    }
}
