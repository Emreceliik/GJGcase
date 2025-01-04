using System.Collections.Generic;
using UnityEngine;

public class BlockController : MonoBehaviour
{
    [Header("Block Properties")]
    public int row;
    public int col;
    public int colorIndex;
    public Sprite[] icons; // Different icons based on group size
    private int Ax, Bx, Cx;

    private SpriteRenderer spriteRenderer;
    private GridManager gridManager;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // GridManager referansını alıyoruz
        gridManager = FindObjectOfType<GridManager>();
        if (gridManager == null)
        {
            Debug.LogError("GridManager not found in the scene!");
        }
        else
        {
            Ax = gridManager.A;
            Bx = gridManager.B;
            Cx = gridManager.C;
        }
    }

    public void UpdateIcon(int groupSize)
    {
        
        if (groupSize <= Ax)
        {
            spriteRenderer.sprite = icons[0]; // Default icon
        }
        else if (groupSize < Bx)
        {
            spriteRenderer.sprite = icons[1]; // First icon
        }
        else if (groupSize < Cx)
        {
            spriteRenderer.sprite = icons[2]; // Second icon
        }
        else
        {
            spriteRenderer.sprite = icons[3]; // Third icon
        }
    }

    public List<Vector2Int> GetConnectedBlocks(GameObject[,] grid, int K, int N)
    {
        List<Vector2Int> connectedBlocks = new List<Vector2Int>();
        Queue<Vector2Int> toCheck = new Queue<Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        Vector2Int startPos = new Vector2Int(row, col);
        if (grid[row, col] == null)
        {
            return connectedBlocks; // Eğer başlangıçta blok yoksa boş liste döndür
        }
        toCheck.Enqueue(startPos);
        visited.Add(startPos);
        while (toCheck.Count > 0)
        {
            Vector2Int current = toCheck.Dequeue();
            connectedBlocks.Add(current);

            foreach (Vector2Int dir in new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
            {
                int newRow = current.x + dir.x;
                int newCol = current.y + dir.y;

                if (newRow >= 0 && newRow < K && newCol >= 0 && newCol < N)
                {
                    Vector2Int neighbor = new Vector2Int(newRow, newCol);

                    if (!visited.Contains(neighbor) && grid[newRow, newCol] != null)
                    {
                        BlockController neighborBlock = grid[newRow, newCol].GetComponent<BlockController>();
                        if (neighborBlock != null && neighborBlock.colorIndex == colorIndex)
                        {
                            toCheck.Enqueue(neighbor);
                            visited.Add(neighbor);
                        }
                    }
                }
            }
        }

        return connectedBlocks;
    }

    public void HighlightBlock(bool highlight)
    {
        spriteRenderer.color = highlight ? Color.yellow : Color.white;
    }

    // **OnMouseDown**: Tıklama olayını yönetir
    void OnMouseDown()
    {
        if (gridManager == null) return;

        GameObject[,] grid = gridManager.GetGrid();
        int M = gridManager.M;
        int N = gridManager.N;

        List<Vector2Int> connectedBlocks = GetConnectedBlocks(grid, M, N);

        if (connectedBlocks.Count > 1)
        {
            gridManager.CollapseBlocks(connectedBlocks);
        }
        gridManager.ButtonFor00();
    }
}
