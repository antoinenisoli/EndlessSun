using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerStat : Stat
{
    public PlayerStatName thisStat;

    public virtual void Init()
    {
        CurrentValue = MaxValue;
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
