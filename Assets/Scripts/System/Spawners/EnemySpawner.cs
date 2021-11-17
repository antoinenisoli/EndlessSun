using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] int spawnCount = 3;
    [SerializeField] Vector2 positionRandomOffset;
    [SerializeField] GameObject enemyPrefab;

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

    public IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);
        Spawn();
    }

    public Vector2 RandomPosition()
    {
        Vector2 circle = Random.insideUnitCircle;
        Vector2 random;
        random.x = circle.x * positionRandomOffset.x;
        random.y = circle.y * positionRandomOffset.y;
        return random;
    }

    public void Spawn()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            Vector2 newPos = transform.position + (Vector3)RandomPosition();
            if (GridManager.Instance)
            {
                bool sampled = GridManager.Instance.SamplePosition(newPos, 2, out Cell cell);
                if (sampled)
                    Instantiate(enemyPrefab, cell.transform.position, Quaternion.identity, transform);
            }
            else
                Instantiate(enemyPrefab, newPos, Quaternion.identity, transform);
        }
    }
}
