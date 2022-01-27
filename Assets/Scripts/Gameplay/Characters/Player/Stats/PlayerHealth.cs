using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerHealth : HealthStat
{
    public override float Coeff()
    {
        return CurrentValue / BaseMaxValue;
    }

    public void Update()
    {
        float computeThirsty = PlayerSurvival.Instance.Thirsty.Coeff();
        MaxValue = BaseMaxValue * computeThirsty;
        if (CurrentValue >= MaxValue)
            CurrentValue = MaxValue;
    }
}
