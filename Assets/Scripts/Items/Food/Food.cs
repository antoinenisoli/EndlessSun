using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Food", menuName = "Items/Food")]
public class Food : ScriptableObject
{
    public string foodName = "Apple";
    public Sprite foodSprite;
    public float feedAmount = 5;
    public PlayerStatName[] changeStats = new PlayerStatName[1];
}