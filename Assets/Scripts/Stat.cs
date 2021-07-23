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

            if (value > maxValue)
                value = maxValue;

            currentValue = value;
        }
    }

    public float MaxValue { get => maxValue; set => maxValue = value; }
}
