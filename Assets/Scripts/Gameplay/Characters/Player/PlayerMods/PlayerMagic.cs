using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerMagic : PlayerMod
{
    public static PlayerStat Mana;
    [SerializeField] PlayerStat mana;

    public override void Init()
    {
        base.Init();
        Mana = mana;
        Mana.Init();
    }

    public override void Update()
    {
        float computeHunger = PlayerSurvival.Instance.Hunger.Difference();
        Mana.MaxValue = Mana.BaseMaxValue - computeHunger;
    }
}
