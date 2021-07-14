using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerXP
{
    [SerializeField] int currentXP;
    [SerializeField] int currentLevel = 0;
    [SerializeField] int powerIncrement = 2;
    [SerializeField] int[] levels = new int[1];

    public int CurrentXP
    {
        get => currentXP;
        set
        {
            if (value < 0)
                value = 0;

            currentXP = value;
        }
    }

    public int MaxLevel => levels.Length;

    public int NextLevelXP => levels[CurrentLevel];

    public int CurrentLevel { get => currentLevel; set => currentLevel = value; }

    public void GenerateLevels()
    {
        for (int i = 1; i < levels.Length; i++)
            levels[i] = levels[i - 1] * powerIncrement;
    }

    public void ModifyValue(int amount)
    {
        CurrentXP += amount;
        if (CurrentXP >= levels[CurrentLevel])
            LevelUp();
    }

    public void LevelUp()
    {
        CurrentLevel++;
        CurrentXP = 0;
    }
}
