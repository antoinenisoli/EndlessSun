using System.Collections;
using UnityEngine;

[System.Serializable]
public struct FeedEffect
{
    public float feedAmount;
    public SurvivalStatName changeStat;
}

[System.Serializable]
public class FoodItem : Item
{
    [Header("FOOD")]
    public FeedEffect[] feedEffects = new FeedEffect[1];

    public override void Effect()
    {
        foreach (var item in feedEffects)
            GameManager.Player.Survival.GetSurvivalStat(item.changeStat).CurrentValue += item.feedAmount;
    }

    public override string ToString()
    {
        string effectText = "";
        foreach (var item in feedEffects)
        {
            effectText += "- Restore " + item.feedAmount + " " + item.changeStat + ".";
            effectText += "\n";
        }

        return Description + "\n" + effectText;
    }
}