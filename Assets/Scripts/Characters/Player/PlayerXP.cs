using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerXP
{
    [SerializeField] PlayerLevel currentLevel = new PlayerLevel();
    [SerializeField] int powerIncrement = 2;
    [SerializeField] PlayerLevel[] levels = new PlayerLevel[1];

    public PlayerLevel MaxLevel => levels[levels.Length - 1];

    public PlayerLevel NextLevel => levels[CurrentLevel.index + 1];

    public PlayerLevel CurrentLevel { get => currentLevel; set => currentLevel = value; }

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
    }
}
