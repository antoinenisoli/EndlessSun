using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StaminaStat : PlayerStat
{
    public float BaseMaxValue;

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

    public override void Init()
    {
        base.Init();
        BaseMaxValue = MaxValue;
    }
}
