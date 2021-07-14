using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XPItem : PickupItem
{
    public int xpAmount;

    public override void Effect(PlayerController2D player)
    {
        player.myXP.ModifyValue(10);
        Destroy(gameObject);
    }
}
