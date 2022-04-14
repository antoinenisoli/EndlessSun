using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/Profiles/Player")]
public class PlayerProfile : CharacterProfile
{
    public SurvivalStat baseHunger, baseStamina, baseThirst;
    public PlayerStat baseEnergy, baseMana, baseHealth;
}
