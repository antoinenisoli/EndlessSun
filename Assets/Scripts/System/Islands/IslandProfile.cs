using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Pathfinding;
using System.IO;
using System.Linq;
using Cinemachine;
using UnityEngine.UI;
using DG.Tweening;

[CreateAssetMenu(fileName = "IslandProfile", menuName = "Regions/Islands/NewIslandProfile")]
public class IslandProfile : ScriptableObject
{
    public IslandBiome biome;
    public RuleTile ruleTile;
    public GameObject enemySpawner;
    public SpawnData spawnData;
    public Color islandColor = Color.white;
    public float spawnerProb = 30f;
}
