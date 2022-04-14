using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimManager : MonoBehaviour
{
    NPC npc;

    private void Awake()
    {
        npc = GetComponentInParent<NPC>();
    }

    public void Attack()
    {
        npc.Attack();
    }

    public void Death()
    {
        Destroy(npc.transform.parent.gameObject);
    }
}
