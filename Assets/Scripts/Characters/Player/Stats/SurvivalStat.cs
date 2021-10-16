using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SurvivalStat : PlayerStat
{
    [Range(0, 0.1f)] public float changeAmount = 0.5f;

    public virtual void Update()
    {
        TimerEffect();
    }

    public virtual void TimerEffect()
    {
        CurrentValue -= changeAmount;
        UIManager.Instance.UpdateUI();
    }
}
