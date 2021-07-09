using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGrid : MonoBehaviour
{
    [SerializeField] Vector2Int gridSize;
    [SerializeField] GameObject cellPrefab;
    [SerializeField] Sprite[] randomVisual;

    private void Awake()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                GameObject newCell = Instantiate(cellPrefab, transform.position + new Vector3(x, y), cellPrefab.transform.rotation, transform);
                newCell.GetComponentInChildren<SpriteRenderer>().sprite = randomVisual[Random.Range(0, randomVisual.Length)];
            }
        }
    }
}
