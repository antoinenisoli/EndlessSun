using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ItemDrop
{
    public GameObject itemPrefab;
    [Range(0,1)] public float prob;
}
