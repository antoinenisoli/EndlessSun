using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerLevel
{
    public int CurrentXP
    {
        get => currentXP;
        set
        {
            if (value < 0)
                value = 0;

            currentXP = value;
            UIManager.Instance.UpdateUI();
        }
    }

    public int index;
    public int xpStep;
    int currentXP;
}
