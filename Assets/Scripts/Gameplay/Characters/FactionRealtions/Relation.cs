using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RelationType
{
    Neutral,
    Friendly,
    Hostile,
}

[System.Serializable]
public struct Relation
{
    public RelationType relationType;
    public Team targetTeam;
}
