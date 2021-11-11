using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerMagic : PlayerMod
{
    public static PlayerStat Mana;
    [SerializeField] PlayerStat _Mana;

    public override void Init()
    {
        base.Init();
        _Mana.Init();
        Mana = _Mana;
    }

    public override void Update()
    {
        float computeHunger = PlayerSurvival.Instance.Hunger.Difference();
        Mana.MaxValue = Mana.BaseMaxValue - computeHunger;
    }
}
