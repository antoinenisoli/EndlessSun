using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerStat : Stat
{
    public float BaseMaxValue { get; set; }
    public override float CurrentValue
    {
        get => base.CurrentValue;
        set
        {
            base.CurrentValue = value;
            if (UIManager.Instance)
                UIManager.Instance.UpdateUI();
        }
    }

    public override float MaxValue
    {
        get => base.MaxValue;
        set
        {
            if (CurrentValue < 0)
                CurrentValue = 0;

            if (CurrentValue > MaxValue)
                CurrentValue = MaxValue;

            maxValue = value;
        }
    }

    public PlayerStatName thisStat;

    public virtual void Init()
    {
        CurrentValue = MaxValue;
        BaseMaxValue = MaxValue;
    }
}