using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAnimManager : MonoBehaviour
{
    NPC npc;

    private void Awake()
    {
        npc = GetComponentInParent<NPC>();
    }

    public void Attack()
    {
        npc.myBehavior.Attack();
    }

    public void Death()
    {
        Destroy(npc.transform.parent.gameObject);
    }
}
