using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Stat
{
    public string statName;
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

    public virtual float MaxValue { get => maxValue; set => maxValue = value; }
}
