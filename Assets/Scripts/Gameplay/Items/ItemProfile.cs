using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemProfile : ScriptableObject
{
    public abstract Item Item { get; }
}
