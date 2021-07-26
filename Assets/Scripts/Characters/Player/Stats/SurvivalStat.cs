using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SurvivalStat : PlayerStat
{
    public float changeAmount = 5;

    public virtual void Update()
    {
        TimerEffect();
    }

    public virtual void TimerEffect()
    {
        CurrentValue += changeAmount;
        UIManager.Instance.UpdateUI();
    }
}
