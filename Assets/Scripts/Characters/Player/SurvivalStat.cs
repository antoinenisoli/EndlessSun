using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SurvivalStat : PlayerStat
{
    public float timerDelay = 5;
    public float looseAmount = 5;
    float timer;

    public void Update()
    {
        timer += Time.deltaTime;
        if (timer > timerDelay)
        {
            CurrentValue -= looseAmount;
            timer = 0;
            UIManager.Instance.UpdateUI();
        }
    }
}
