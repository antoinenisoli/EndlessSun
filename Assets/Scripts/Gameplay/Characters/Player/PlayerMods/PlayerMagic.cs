using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerMagic : PlayerMod
{
    public static CharacterStat Mana;
    [SerializeField] CharacterStat mana;

    public override void Init()
    {
        base.Init();
        PlayerController2D.Magic = this;
        Mana = mana;
        Mana.Init(player);
    }

    public override void DoUpdate()
    {
        float computeHunger = PlayerSurvival.Instance.Hunger.Difference();
        Mana.MaxValue = Mana.BaseMaxValue - computeHunger;
    }
}
