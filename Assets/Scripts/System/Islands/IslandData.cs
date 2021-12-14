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

[CreateAssetMenu(fileName = "IslandData", menuName = "Regions/Islands/NewIslandData")]
public class IslandData : ScriptableObject
{
    public IslandBiome biome;
    public RuleTile ruleTile;
    public GameObject enemySpawner;
    public Color islandColor = Color.white;
    public float spawnerProb = 30f;
}
