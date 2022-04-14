using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/Profiles/Player")]
public class PlayerProfile : CharacterProfile
{
    [Header("_PLAYER")]
    public StaminaStat Stamina;
    public SurvivalStat baseHunger;
    public SurvivalStat baseStamina;
    public SurvivalStat baseThirst;
}
