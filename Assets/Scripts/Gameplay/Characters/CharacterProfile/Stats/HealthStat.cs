using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HealthStat : FullStat
{
    public bool isDead;

    public override float CurrentValue 
    { 
        get => base.CurrentValue; 
        set
        {
            if (value <= 0 && !isDead)
                isDead = true;

            base.CurrentValue = value;
        }
    }

    public override void Init(Entity entity)
    {
        base.Init(entity);
        ComputeDurability();
        CurrentValue = MaxValue;
    }

    public void ModifyValue(float amount)
    {
        CurrentValue += amount;
    }

    public void ComputeDurability()
    {
        float value = (float)entity.CharacterProfile.AttributeList.Durability.value * ((float)MaxValue * 0.1f);
        MaxValue += value;
    }
}
