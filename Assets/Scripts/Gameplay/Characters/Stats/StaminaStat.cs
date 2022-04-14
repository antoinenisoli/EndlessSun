using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StaminaStat : FullStat
{
    [SerializeField] float recoveryDelay = 2;
    [SerializeField] float recoveryAmount = 0.01f;
    float recoveryTimer;
    bool isRecovering;

    public override float CurrentValue 
    { 
        get => base.CurrentValue; 
        set
        {
            if (value >= MaxValue)
            {
                isRecovering = false;
                value = MaxValue;
            }

            base.CurrentValue = value;
        }
    }

    public void Update()
    {
        if (CurrentValue < MaxValue)
        {
            if (!isRecovering)
            {
                recoveryTimer += Time.deltaTime;
                if (recoveryTimer > recoveryDelay)
                {
                    recoveryTimer = 0;
                    isRecovering = true;
                }
            }
            else
            {
                CurrentValue += recoveryAmount;
                UIManager.Instance.UpdateUI();
            }
        }
        else
        {
            recoveryTimer = 0;
        }
    }

    public void StaminaCost(float amount)
    {
        CurrentValue -= amount;
        StopRecovery();
    }

    public void StopRecovery()
    {
        recoveryTimer = 0;
        isRecovering = false;
    }
}
