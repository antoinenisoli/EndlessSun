using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Food", menuName = "Items/Food")]
public class FoodItemProfile : ItemProfile
{
    public FoodItem FoodItem;

    public override Item Item => FoodItem;
}
