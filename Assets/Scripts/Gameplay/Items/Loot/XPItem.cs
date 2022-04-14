using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XPItem : AutoCollectItem
{
    [Header("XP ITEM")]
    public int xpAmount;

    public override void Effect(PlayerController2D player)
    {
        PlayerController2D.xpManager.ModifyValue(10);
        player.StartCoroutine(player.Glow(0.1f, Color.white));
        VFXManager.Instance.PlayVFX("XPCoin", transform.position);
        Destroy(gameObject);
    }

    public override void Interact()
    {
        
    }
}
