using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerStatName
{
    Health,
    Mana,
    Stamina,
    Hunger,
    Thirst,
    Energy,
}

[System.Serializable]
public class PlayerSurvival
{
    public static SurvivalStat Hunger;
    public static SurvivalStat Thirsty;
    public static SurvivalStat Energy;

    public SurvivalStat[] stats = new SurvivalStat[3];
    Dictionary<PlayerStatName, SurvivalStat> dico = new Dictionary<PlayerStatName, SurvivalStat>();

    public void Init()
    {
        foreach (var item in stats)
        {
            dico.Add(item.thisStat, item);
            item.CurrentValue = item.MaxValue;
        }

        Hunger = dico[PlayerStatName.Hunger];
        Thirsty = dico[PlayerStatName.Thirst];
        Energy = dico[PlayerStatName.Energy];
    }

    public void Update()
    {
        foreach (var item in stats)
            item.Update();
    }
}
