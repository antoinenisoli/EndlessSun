using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGrid : MonoBehaviour
{
    [SerializeField] Vector2Int gridSize;
    [SerializeField] GameObject cellPrefab;
    [SerializeField] Sprite[] randomVisual;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, (Vector2)gridSize);
    }

    private void Awake()
    {
        for (int x = -gridSize.x/2; x < gridSize.x/2; x++)
        {
            for (int y = -gridSize.y / 2; y < gridSize.y/2; y++)
            {
                GameObject newCell = Instantiate(cellPrefab, transform.position + new Vector3(x, y), cellPrefab.transform.rotation, transform);
                newCell.GetComponentInChildren<SpriteRenderer>().sprite = randomVisual[Random.Range(0, randomVisual.Length)];
            }
        }
    }
}
