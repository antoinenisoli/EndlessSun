using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimManager : MonoBehaviour
{
    Enemy enemy;

    private void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
    }

    public void Attack()
    {
        enemy.Attack();
    }

    public void Death()
    {
        Destroy(enemy.transform.parent.gameObject);
    }
}
