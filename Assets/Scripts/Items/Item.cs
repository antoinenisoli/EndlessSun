using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : PickupItem
{
    public override void Effect(PlayerController2D player)
    {
        Destroy(gameObject);
    }
}
