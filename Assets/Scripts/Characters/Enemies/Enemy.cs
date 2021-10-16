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

    [SerializeField] float minDistance = 1f;

    public override void Start()
    {
        base.Start();
        healthBarSprites = healthBar.GetComponentsInChildren<SpriteRenderer>();
    }

    public override void Hit(float amount, Vector2 force = default)
    {
        base.Hit(amount, force);
        stunned = true;
        StartCoroutine(Unstun());
        healthBarPivot.DOScaleX((float)health.CurrentValue / (float)health.MaxValue, 0.3f);
    }

    public virtual void Attack(Entity target)
    {
        
    }

    IEnumerator Unstun()
    {
        yield return new WaitForSeconds(0.45f);
        stunned = false;
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

    public override void Update()
    {
        base.Update();
        alpha = Mathf.Lerp(alpha, health.CurrentValue < health.MaxValue ? 1 : 0, 15 * Time.deltaTime);
        foreach (var item in healthBarSprites)
        {
            Color col = item.color;
            col.a = alpha;
            item.color = col;
        }

        if (GameManager.Player && !health.isDead)
        {
            spr.flipX = transform.position.x > GameManager.Player.transform.position.x;
            float distance = Vector2.Distance(GameManager.Player.transform.position, transform.position);
            if (distance > minDistance)
                rb.AddForce((GameManager.Player.transform.position - transform.position).normalized * baseSpeed * Time.deltaTime);
        }
    }
}
