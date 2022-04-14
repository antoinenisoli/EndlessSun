using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AttributeList
{
    Dictionary<AttributeType, CharacterAttribute> _attributes = new Dictionary<AttributeType, CharacterAttribute>();
    #region "Create attributes"
    [SerializeField] CharacterAttribute[] attributeList = new CharacterAttribute[]
    {
        new CharacterAttribute(AttributeType.Strength, 5),
        new CharacterAttribute(AttributeType.Durability, 6),
        new CharacterAttribute(AttributeType.Endurance, 3),
        new CharacterAttribute(AttributeType.Knowledge, 2),
        new CharacterAttribute(AttributeType.Speed, 5),
    };
    #endregion

    #region "Get attributes"
    public CharacterAttribute Strength => GetAttribute(AttributeType.Strength);
    public CharacterAttribute Durability => GetAttribute(AttributeType.Durability);
    public CharacterAttribute Endurance => GetAttribute(AttributeType.Endurance);
    public CharacterAttribute Knowledge => GetAttribute(AttributeType.Knowledge);
    public CharacterAttribute Speed => GetAttribute(AttributeType.Speed);
    #endregion

    public AttributeList(CharacterAttribute[] attributeList)
    {
        this.attributeList = attributeList;
    }

    public void Init()
    {
        _attributes = new Dictionary<AttributeType, CharacterAttribute>();
        foreach (var item in attributeList)
            _attributes.Add(item.attributeType, item);
    }

    public CharacterAttribute GetAttribute(AttributeType type)
    {
        if (_attributes.TryGetValue(type, out CharacterAttribute attribute))
            return attribute;
        else
            return null;
    }

    public int ComputeDamages()
    {
        int compute = Mathf.RoundToInt((float)50 / (float)100 * (float)Strength.value);
        return compute;
    }

    public bool BalanceDraw(Entity target)
    {
        System.Random rdm = new System.Random();
        int draw = rdm.Next(0, 50);
        int computeResistance = target.AttributeList.Strength.value + Durability.value;
        return draw > computeResistance;
    }

    public AttributeList Copy()
    {
        return new AttributeList(attributeList);
    }
}
