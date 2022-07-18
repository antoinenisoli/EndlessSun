using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RelationManager
{
    public Team myTeam;
    public Entity myEntity;
    public Relation[] mainRelations;
    Dictionary<Team, Relation> myRelations = new Dictionary<Team, Relation>();

    public void Init()
    {
        foreach (var item in mainRelations)
            myRelations.Add(item.targetTeam, item);
    }

    public RelationType GetRelation(Team otherTeam)
    {
        if (myRelations.TryGetValue(otherTeam, out Relation relation))
            return relation.relationType;

        return RelationType.Neutral;
    }

    public bool CheckRelation(RelationType relationType, Entity other)
    {
        return GetRelation(other.relationManager.myTeam) == relationType;
    }
}
