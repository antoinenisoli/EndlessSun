using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SurvivalStatName
{
    Hunger,
    Thirst,
    Energy,
}

public class PlayerSurvival : MonoBehaviour
{
    public static PlayerSurvival Instance;
    [SerializeField] SurvivalStat hunger;
    [SerializeField] SurvivalStat thirsty;
    [SerializeField] SurvivalStat energy;
    Dictionary<SurvivalStatName, SurvivalStat> d_stats = new Dictionary<SurvivalStatName, SurvivalStat>();

    public SurvivalStat Hunger { get => hunger; set => hunger = value; }
    public SurvivalStat Thirsty { get => thirsty; set => thirsty = value; }
    public SurvivalStat Energy { get => energy; set => energy = value; }

    private void Awake()
    {
        if (!Instance)
            Instance = this;

        Init();
    }

    public void Init()
    {
        d_stats.Add(Hunger.thisStat, Hunger);
        d_stats.Add(Thirsty.thisStat, Thirsty);
        d_stats.Add(Energy.thisStat, Energy);
        foreach (var item in d_stats.Values)
            item.Init(GameManager.Player);
    }

    public SurvivalStat GetSurvivalStat(SurvivalStatName statName)
    {
        if (d_stats.TryGetValue(statName, out SurvivalStat stat))
            return stat;

        return null;
    }

    public void Update()
    {
        foreach (var item in d_stats.Values)
            item.Update();
    }
}
