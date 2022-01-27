using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public string Name;
    [TextArea] public string Description;
    public Sprite Sprite;

    public virtual void Effect() { }
}
