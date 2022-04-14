using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Stat
{
    protected PlayerController2D player => GameManager.Player;
    [SerializeField] protected float currentValue;
    [SerializeField] protected float maxValue = 50; 

    public virtual float CurrentValue
    {
        get => currentValue;
        set
        {
            if (value < 0)
                value = 0;

            if (value > MaxValue)
                value = MaxValue;

            currentValue = value;
        }
    }

    public float Difference()
    {
        return MaxValue - CurrentValue;
    }

    public virtual float Coeff() { return CurrentValue / MaxValue; }

    public virtual float MaxValue { get => maxValue; set => maxValue = value; }
}
