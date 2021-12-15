using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] Vector2 positionRandomOffset;
    [SerializeField] SpawnData spawnerData;

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        Handles.color = Color.red;
        Handles.DrawWireDisc(transform.position, Vector3.forward, positionRandomOffset.y);

        Color c = Color.red;
        c.a = 0.2f;
        Handles.color = c;
        Handles.DrawSolidDisc(transform.position, Vector3.forward, positionRandomOffset.x);
#endif
    }

    public void Awake()
    {
        if (spawnerData)
            Spawn(spawnerData);
    }

    private void Start()
    {
        Vector2 pos = transform.position;
        if (GridManager.Instance)
        {
            Vector2 sampled = GridManager.Instance.ClosestWalkable(pos);
            transform.position = sampled;
        }
    }

    public Vector2 RandomPosition()
    {
        Vector2 circle = Random.insideUnitCircle;
        Vector2 random;
        random.x = circle.x * positionRandomOffset.x;
        random.y = circle.y * positionRandomOffset.y;
        return random;
    }

    public void Spawn(SpawnData data)
    {
        if (spawnerData != data)
            spawnerData = data;

        GameObject[] enemies = data.GetSpawnArray();
        for (int i = 0; i < enemies.Length; i++)
        {
            Vector2 newPos = transform.position + (Vector3)RandomPosition();
            if (GridManager.Instance)
            {
                //bool sampled = GridManager.Instance.SamplePosition(newPos, out Vector2 samplePos, 2);
                Instantiate(enemies[i], newPos, Quaternion.identity, transform);
            }
            else
                Instantiate(enemies[i], newPos, Quaternion.identity, transform);
        }

        spawnerData = null;
    }
}
