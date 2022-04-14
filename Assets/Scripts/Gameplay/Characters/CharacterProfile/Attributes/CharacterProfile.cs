using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/Profiles/Common")]
public class CharacterProfile : ScriptableObject
{
    [Header("_CharacterProfile")]
    public AttributeList AttributeList;
    public FullStat Health, Mana;
    public CharacterStat[] stats = new CharacterStat[3];
}
