using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGrid : MonoBehaviour
{
    [SerializeField] Vector2Int gridSize;
    [SerializeField] GameObject cellPrefab;
    [SerializeField] Sprite[] randomVisual;
    Cell[,] allCells = new Cell[0,0];

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, (Vector2)gridSize);
    }

    private void Awake()
    {
        CreateGrid();
        GetGridBorder();
    }

    void CreateGrid()
    {
        allCells = new Cell[gridSize.x, gridSize.y];
        for (int x = -gridSize.x / 2; x < gridSize.x / 2; x++)
        {
            for (int y = -gridSize.y / 2; y < gridSize.y / 2; y++)
            {
                GameObject newCell = Instantiate(cellPrefab, transform.position + new Vector3(x, y), cellPrefab.transform.rotation, transform);
                Cell cellScript = newCell.GetComponent<Cell>();
                Vector2Int yes = new Vector2Int(x, y) + gridSize / 2;
                allCells[yes.x, yes.y] = cellScript;
                newCell.name = cellPrefab.name + " [" + yes + "] ";
                newCell.GetComponentInChildren<SpriteRenderer>().sprite = randomVisual[Random.Range(0, randomVisual.Length)];
            }
        }
    }

    void GetGridBorder()
    {
        List<Cell> borderCells = new List<Cell>();
        for (int i = 0; i < allCells.GetLength(0); i++)
        {
            Cell obj = allCells[i, 0];
            if (!borderCells.Contains(obj))
                borderCells.Add(obj);
        }

        for (int i = 0; i < allCells.GetLength(0); i++)
        {
            Cell obj = allCells[i, allCells.GetLength(1) - 1];
            if (!borderCells.Contains(obj))
                borderCells.Add(obj);
        }

        for (int i = 0; i < allCells.GetLength(1); i++)
        {
            Cell obj = allCells[0, i];
            if (!borderCells.Contains(obj))
                borderCells.Add(obj);
        }

        for (int i = 0; i < allCells.GetLength(1); i++)
        {
            Cell obj = allCells[allCells.GetLength(0) - 1, i];
            if (!borderCells.Contains(obj))
                borderCells.Add(obj);
        }

        foreach (var item in borderCells)
            item.SetType(CellType.Water);
    }
}
