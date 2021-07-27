using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Enemy : Entity
{
    [Header("ENEMY")]
    [SerializeField] Transform healthBarPivot;
    [SerializeField] Transform healthBar;
    SpriteRenderer[] healthBarSprites;
    [SerializeField] GameObject xpPrefab;
    [SerializeField] int xpAmount = 25;
    [SerializeField] ItemDrop[] loots;
    float alpha;

    public override void Start()
    {
        base.Start();
        healthBarSprites = healthBar.GetComponentsInChildren<SpriteRenderer>();
    }

    public override void Hit(float amount, Vector2 force = default)
    {
        base.Hit(amount, force);
        healthBarPivot.DOScaleX((float)health.CurrentValue / (float)health.MaxValue, 0.3f);
    }

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

        Destroy(gameObject, 2f);
    }

    public void Update()
    {
        alpha = Mathf.Lerp(alpha, health.CurrentValue < health.MaxValue ? 1 : 0, 15 * Time.deltaTime);
        foreach (var item in healthBarSprites)
        {
            Color col = item.color;
            col.a = alpha;
            item.color = col;
        }
    }
}
