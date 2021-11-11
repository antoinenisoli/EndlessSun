using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttributeType
{
    Strength,
    Durability,
    Endurance,
    Knowledge,
    Speed,
}

[System.Serializable]
public class CharacterAttribute
{
    public string name;
    public AttributeType attributeType;
    public int value;
    readonly List<StatModifier> statModifiers;

    public CharacterAttribute(AttributeType attributeType, int value)
    {
        this.attributeType = attributeType;
        this.value = value;
        name = attributeType.ToString();
        statModifiers = new List<StatModifier>();
    }
}
