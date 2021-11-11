using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AttributeList
{
    Dictionary<AttributeType, CharacterAttribute> _attributes;
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

    public void Init()
    {
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
        return (int)Mathf.Round(50 / 100 * (float)Strength.value);
    }

    public bool BalanceDraw(Entity target)
    {
        System.Random rdm = new System.Random();
        int draw = rdm.Next(0, 50);
        int computeResistance = target.AttributeList.Strength.value + Durability.value;
        return draw > computeResistance;
    }
}
