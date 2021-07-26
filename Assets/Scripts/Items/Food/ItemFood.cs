using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFood : Item
{
    [SerializeField] Food myFood;

    public override void Effect(PlayerController2D player)
    {
        foreach (var item in myFood.changeStats)
        {
            GameManager.Player.Survival.GetSurvivalStat(item).CurrentValue += myFood.feedAmount;
        }
    }
}
