using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HealthStat : PlayerStat
{
    public bool isDead => CurrentValue <= 0;

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
