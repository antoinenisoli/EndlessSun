using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerHealth : HealthStat
{
    public void Update()
    {
        float computeThirsty = BaseMaxValue - (PlayerSurvival.Instance.Thirsty.Difference() / BaseMaxValue);
        MaxValue = computeThirsty;
        if (CurrentValue < MaxValue)
            CurrentValue = MaxValue;
    }
}
