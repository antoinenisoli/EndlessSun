using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Health : PlayerStat
{
    public bool isDead => CurrentValue <= 0;

    public void Initialize()
    {
        CurrentValue = MaxValue;
    }

    public void ModifyValue(float amount)
    {
        CurrentValue += amount;
    }
}
