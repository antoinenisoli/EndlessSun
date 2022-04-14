using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SurvivalStat : FullStat
{
    PlayerController2D player => entity as PlayerController2D;

    [Range(0, 100f)] public float changeAmount = 0.5f;
    float timer;
    public SurvivalStatName thisStat;

    public virtual void Update()
    {
        if (timer > 2)
            TimerEffect();
        else
            timer += Time.deltaTime;
    }

    public virtual void TimerEffect()
    {
        timer = 0;
        CurrentValue -= changeAmount;
        UIManager.Instance.UpdateUI();

        if (!player.Health.isDead && CurrentValue <= 0)
            player.Death();
    }
}
