using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] Vector2 positionRandomOffset;
    [SerializeField] SpawnData spawnerData;
    [SerializeField] bool active = true;
    [SerializeField] ShowRectangleGizmo gizmo;

    [SerializeField] float sampleDistance = 2f;
    List<Vector2> sampledPositions = new List<Vector2>();

    private void OnDrawGizmos()
    {
        if (gizmo)
            gizmo.SetSize(positionRandomOffset * 2);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        foreach (var item in sampledPositions)
        {
            Gizmos.DrawLine(transform.position, item);
            Gizmos.DrawWireSphere(item, 0.6f);
        }
    }

    public void Awake()
    {
        if (spawnerData && active)
            Spawn(spawnerData);
    }

    public Vector2 RandomPosition() => GameDevHelper.RandomVector(positionRandomOffset, transform.position);

    public void Spawn(SpawnData data)
    {
        if (spawnerData != data)
            spawnerData = data;

        GameObject[] enemies = data.GetSpawnArray();
        sampledPositions.Clear();
        for (int i = 0; i < enemies.Length; i++)
        {
            Vector2 newPos = RandomPosition();
            if (GridManager.Instance)
            {
                bool sampled = GridManager.Instance.SamplePosition(newPos, 2, out Vector2 sampledPos);
                if (sampled)
                {
                    sampledPositions.Add(sampledPos);
                    Instantiate(enemies[i], newPos, Quaternion.identity, transform);
                }
            }
            else
                Instantiate(enemies[i], newPos, Quaternion.identity, transform);
        }

        spawnerData = null;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            sampledPositions.Clear();
            for (int i = 0; i < 10; i++)
            {
                Vector2 newPos = RandomPosition();
                if (GridManager.Instance)
                {
                    bool sampled = GridManager.Instance.SamplePosition(newPos, sampleDistance, out Vector2 sampledPos);
                    if (sampled)
                        sampledPositions.Add(sampledPos);
                }
            }
        }
    }
}
