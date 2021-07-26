using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerStat : Stat
{
    public float BaseMaxValue { get; set; }

    public PlayerStatName thisStat;

    public virtual void Init()
    {
        CurrentValue = MaxValue;
        BaseMaxValue = MaxValue;
    }

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
}
