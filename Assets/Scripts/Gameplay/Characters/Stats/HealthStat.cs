using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HealthStat : PlayerStat
{
    public bool isDead;

    public override float CurrentValue 
    { 
        get => base.CurrentValue; 
        set
        {
            base.CurrentValue = value;
            if (value <= 0 && !isDead)
                isDead = true;
        }
    }

    public override void Init()
    {
        base.Init();
        CurrentValue = MaxValue;
    }

    public void ModifyValue(float amount)
    {
        CurrentValue += amount;
    }
}
