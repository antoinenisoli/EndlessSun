using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterStatName
{
    Health,
    Mana,
    Stamina,
}

public class CharacterManager : CharacterMod
{
    [SerializeField] PlayerLevel currentLevel = new PlayerLevel();
    [SerializeField] int powerIncrement = 2;
    [SerializeField] PlayerLevel[] levels = new PlayerLevel[1];
    Dictionary<CharacterStatName, CharacterStat> dico = new Dictionary<CharacterStatName, CharacterStat>();

    public PlayerLevel MaxLevel => levels[levels.Length - 1];

    public PlayerLevel NextLevel => levels[CurrentLevel.index + 1];

    public PlayerLevel CurrentLevel { get => currentLevel; set => currentLevel = value; }

    public override void Init()
    {
        base.Init();
        GetStats();
        GenerateLevels();
    }

    public CharacterStat GetStat(CharacterStatName statName)
    {
        if (dico.TryGetValue(statName, out CharacterStat stat))
            return stat;

        return null;
    }

    void GetStats()
    {
        foreach (var item in myCharacter.CharacterProfile.stats)
        {
            dico.Add(item.thisStat, item);
            item.Init(entity);
        }
    }

    public void GenerateLevels()
    {
        if (levels.Length > 0)
        {
            levels[0].xpStep = CurrentLevel.xpStep;

            for (int i = 1; i < levels.Length; i++)
            {
                levels[i].xpStep = levels[i - 1].xpStep * powerIncrement;
                levels[i].index = i;
            }
        }
    }

    public float ComputeXP()
    {
        float computeXP = (float)PlayerController2D.xpManager.CurrentLevel.CurrentXP / (float)PlayerController2D.xpManager.CurrentLevel.xpStep;
        return computeXP;
    }

    public void ModifyValue(int amount)
    {
        CurrentLevel.CurrentXP += amount;
        if (CurrentLevel.CurrentXP >= CurrentLevel.xpStep)
        {
            CurrentLevel.CurrentXP = CurrentLevel.xpStep;
            LevelUp();
        }
    }

    public void LevelUp()
    {
        CurrentLevel = NextLevel;
        EventManager.Instance.onLevelUp.Invoke();
        GameManager.Player.LevelUp();
    }
}
