using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnInfo
{
    public string name;
    public GameObject prefab;
    [Range(0,100)] public int probability;
    [HideInInspector] public double weight;

    public virtual string Print()
    {
        return
            "<color=red>●</color> Spawn : " +
                "<color=red>[" + prefab.name + "]</color>, " +
                "proba : <b>" + probability + "</b>%";
    }
}

[CreateAssetMenu(fileName = "SpawnerData", menuName = "Spawns/NewSpawnerData")]
public class SpawnData : ScriptableObject
{
    public int spawnCount = 3;
    public SpawnInfo[] spawnInfos;
    public double accumulatedWeights;
    System.Random rand = new System.Random();

    public SpawnInfo RandomSpawn()
    {
        return spawnInfos[GetRandomIndex()];
    }

    public void CalculateWeights()
    {
        accumulatedWeights = 0;
        foreach (var info in spawnInfos)
        {
            accumulatedWeights += info.probability;
            info.weight = accumulatedWeights;
        }
    }

    private void OnValidate()
    {
        foreach (var info in spawnInfos)
            if (info.prefab)
                info.name = info.prefab.name;
    }

    int GetRandomIndex()
    {
        double r = rand.NextDouble() * accumulatedWeights;
        for (int i = 0; i < spawnInfos.Length; i++)
        {
            if (spawnInfos[i].weight >= r)
                return i;
        }

        return 0;
    }

    public GameObject[] GetSpawnArray()
    {
        GameObject[] spawns = new GameObject[spawnCount];
        for (int i = 0; i < spawnCount; i++)
        {
            CalculateWeights();
            SpawnInfo spawnInfo = RandomSpawn();
            spawns[i] = spawnInfo.prefab;
            //Debug.Log(spawnInfo.Print());
        }

        return spawns;
    }
}
