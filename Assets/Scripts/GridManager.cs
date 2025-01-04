using System.Collections.Generic;
using System.Collections; // IEnumerator ve Coroutine kullanımı için gereklidir
using System.Linq;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    [Range(2, 10)]

    public int M;//ROWS
    [Range(2, 10)]

    public int N;//COLUMNS


    public int A, B, C;
    public int minumumGroupSize = 2;

    [Header("Block Prefabs")]
    [Tooltip("Set the number of block prefabs between 1 and 6.")]
    public GameObject[] blockPrefabs; // Prefabs for each block type

    private GameObject[,] grid;

    void Start()
    {
        // Renkleri 1 ve 6 arasında sınırlama
        if (blockPrefabs.Length < 1 || blockPrefabs.Length > 6)
        {
            Debug.LogError("blockPrefabs array must have between 1 and 6 elements.");
            return;
        }
        InitializeGrid();
        Debug.Log("GridManager");
        ShuffleBoard();
    }

    // ButtonFor fonksiyonu, belirli bir hücre için işlem yapar
    public void ButtonFor(int row, int col)
    {
        var blockController = grid[row, col]?.GetComponent<BlockController>();
        if (blockController != null)
        {
            var connectedBlocks = GetConnectedBlocks(row, col, blockController.colorIndex, new HashSet<Vector2Int>());

            Debug.Log($"Connected blocks count: {connectedBlocks.Count}");

            if (connectedBlocks.Count >= minumumGroupSize)
            {

                ChangeSpriteBlocks(connectedBlocks);
            }
            else
            {

            }
        }
    }

    // ButtonFor00 fonksiyonu, tüm hücrelere işlem uygular
    public void ButtonFor00()
    {

        //var blockController = grid[N, M].GetComponent<BlockController>();
        for (int i = 0; i < M; i++)
        {
            for (int j = 0; j < N; j++)
            {
                ButtonFor(i, j);
                //blockController.UpdateIcon(minumumGroupSize);


            }
        }
    }
    // Tüm blokların sprite'ını değiştiren fonksiyon
    public void UpdateAllBlocksSprite()
    {
        // Grid üzerindeki tüm blokları gez
        for (int row = 0; row < M; row++)
        {
            for (int col = 0; col < N; col++)
            {
                // Eğer bu hücrede bir blok varsa
                if (grid[row, col] != null)
                {
                    // Blok controller'ını al
                    var blockController = grid[row, col].GetComponent<BlockController>();

                    // Burada bağlantıya bakılmadan tüm blokların sprite'ını güncelleriz
                    blockController.UpdateIcon(minumumGroupSize);
                }
            }
        }
    }

    // ChangeSpriteBlocks fonksiyonu, bir gruptaki tüm blokların görünümünü günceller
    public void ChangeSpriteBlocks(List<Vector2Int> group)
    {
        foreach (var position in group)
        {
            if (grid[position.x, position.y] != null)
            {
                var blockController = grid[position.x, position.y].GetComponent<BlockController>();
                blockController.UpdateIcon(group.Count);
            }
        }
    }

    // CheckForDeadlock fonksiyonu, sıkışma olup olmadığını kontrol eder
    public bool CheckForDeadlock()
    {
        for (int row = 0; row < M; row++)
        {
            for (int col = 0; col < N; col++)
            {
                if (IsPartOfGroup(row, col))
                {
                    return false;
                }
            }
        }
        return true;
    }

    // CollapseBlocks fonksiyonu, grup halinde çöken blokları yok eder
    // CollapseBlocks fonksiyonu
    public void CollapseBlocks(List<Vector2Int> group)
    {
        if (group.Count < minumumGroupSize)
        {
            Debug.Log($"No valid group to collapse. Group size is less than {minumumGroupSize}.");
            return;
        }

        foreach (var position in group)
        {
            if (grid[position.x, position.y] != null)
            {
                StartCoroutine(SmoothDestroy(grid[position.x, position.y])); // Smooth yok etme animasyonu
                grid[position.x, position.y] = null;
            }
        }
        DropBlocks();
        FillEmptySpaces();
        ButtonFor00();

        if (CheckForDeadlock())
        {
            ResetGrid();

        }
    }

    // Smooth yok etme animasyonu
    private IEnumerator SmoothDestroy(GameObject block)
    {
        // Küçülme animasyonu (0.3 saniye boyunca)
        Vector3 originalScale = block.transform.localScale;
        float timeElapsed = 0f;
        float duration = 0.3f;

        // Küçültme işlemi
        while (timeElapsed < duration)
        {
            block.transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Tamamen yok olunca yok etme işlemi
        block.transform.localScale = Vector3.zero; // Son olarak sıfır yap
        Destroy(block);
    }


    // CollapseBlocksInColumns fonksiyonu, tüm kolonlar için çökertme işlemi yapar
    public void CollapseBlocksInColumns()
    {
        List<List<Vector2Int>> allGroups = new List<List<Vector2Int>>();

        for (int col = 0; col < N; col++)
        {
            HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

            for (int row = 0; row < M; row++)
            {

                if (grid[row, col] != null && !visited.Contains(new Vector2Int(row, col)))
                {
                    visited.Add(new Vector2Int(row, col));

                    var blockController = grid[row, col].GetComponent<BlockController>();
                    var connectedBlocks = GetConnectedBlocks(row, col, blockController.colorIndex, visited);

                    if (connectedBlocks.Count >= 2)
                    {
                        allGroups.Add(connectedBlocks);
                    }
                }
            }
        }

        foreach (var group in allGroups)
        {
            CollapseBlocks(group);

        }
    }

    // DropBlocks fonksiyonu, blokları boşlukları dolduracak şekilde düşürür
    void DropBlocks()
    {
        // Blokları düşürme işlemi
        for (int col = 0; col < N; col++)
        {
            int emptyRow = -1;

            for (int row = 0; row < M; row++)
            {
                if (grid[row, col] == null && emptyRow == -1)
                {
                    emptyRow = row;
                }
                else if (grid[row, col] != null && emptyRow != -1)
                {
                    grid[emptyRow, col] = grid[row, col];
                    grid[row, col] = null;

                    StartCoroutine(MoveBlockSmoothly(grid[emptyRow, col], new Vector2(col, -emptyRow)));

                    var blockController = grid[emptyRow, col].GetComponent<BlockController>();
                    blockController.row = emptyRow;

                    emptyRow++;
                }
            }
        }

        // Kaydırma işlemi sonrasında tüm blokların sprite'larını güncelle
        UpdateAllBlocksSprite();
    }


    IEnumerator MoveBlockSmoothly(GameObject block, Vector2 targetPosition)
    {
        // Eğer block ya da block.transform null ise, deadlock olduğunu belirtiyoruz
        if (block == null || block.transform == null)
        {
            Debug.Log("Deadlock detected");
            yield break; // Fonksiyonu sonlandır
        }

        float time = 0f;
        Vector2 startPosition = block.transform.position;
        float duration = 0.5f;  // Düşme süresi, burada 0.5 saniye belirledik

        while (time < duration)
        {
            if (block == null || block.transform == null)
            {
                Debug.Log("Deadlock detected");
                yield break; // Fonksiyonu sonlandır
            }
            block.transform.position = Vector2.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;

        }
        // Son pozisyonu kesinleştir
        block.transform.position = targetPosition;
    }



    // FillEmptySpaces fonksiyonu, grid'deki boş hücreleri doldurur
    void FillEmptySpaces()
    {
        for (int col = 0; col < N; col++)
        {
            for (int row = 0; row < M; row++)
            {
                if (grid[row, col] == null)
                {
                    StartCoroutine(SpawnBlockWithAnimation(row, col));
                }
            }
        }
    }
    IEnumerator SpawnBlockWithAnimation(int row, int col)
    {
        int randomColorIndex = Random.Range(0, blockPrefabs.Length);
        Vector2 spawnPosition = new Vector2(col, -M);  // Başlangıç pozisyonunu daha yukarı alıyoruz (yani `M` kadar)
        GameObject block = Instantiate(blockPrefabs[randomColorIndex], spawnPosition, Quaternion.identity, transform);
        grid[row, col] = block;

        var blockController = block.GetComponent<BlockController>();
        blockController.row = row;
        blockController.col = col;

        float time = 0f;
        Vector2 targetPosition = new Vector2(col, -row);  // Hedef pozisyon, düşürmek istediğimiz yer

        // Blokları smooth bir şekilde kaydır
        float duration = 0.5f;  // 0.5 saniye boyunca hareket etsin

        while (time < duration)
        {
            if (block == null)
            {
                Debug.Log("Deadlock Oldu");
                break;  // Erişim sağlanamazsa döngüden çık
            }
            block.transform.position = Vector2.Lerp(spawnPosition, targetPosition, time / duration);
            time += Time.deltaTime;

            yield return null;

        }

        // Son pozisyonu kesinleştir
        if (block != null)
        {
            block.transform.position = targetPosition;  // Son pozisyonu kesinleştir
        }
        else
        {
            Debug.Log("Deadlock Oldu");
        }
    }

    public void ResetGrid()
    {
        Debug.Log("Resetting grid due to persistent deadlock.");

        // Tüm blokları yok et
        for (int row = 0; row < M; row++)
        {
            for (int col = 0; col < N; col++)
            {
                if (grid[row, col] != null)
                {
                    Destroy(grid[row, col]);
                    grid[row, col] = null;
                }
            }
        }

        // Yeni grid oluştur
        InitializeGridWithGuaranteedMatches();
        ShuffleBoard(); // Karıştırma işlemi
    }
    void InitializeGridWithGuaranteedMatches()
    {
        grid = new GameObject[M, N];

        // İlk olarak 2x2'lik alanları aynı blokla doldur
        for (int row = 0; row < M - 1; row += 2)  // Adım adım 2'şer artarak ilerliyoruz, son satırı atlıyoruz
        {
            for (int col = 0; col < N - 1; col += 2)  // Yine 2'şer artarak ilerliyoruz, son sütunu atlıyoruz
            {
                int colorIndex = Random.Range(0, blockPrefabs.Length); // Rastgele bir renk seçiyoruz
                // 2x2'lik alanı aynı renk ile doldur
                SpawnBlockWithGuaranteedColor(row, col, colorIndex);
                SpawnBlockWithGuaranteedColor(row + 1, col, colorIndex);
                SpawnBlockWithGuaranteedColor(row, col + 1, colorIndex);
                SpawnBlockWithGuaranteedColor(row + 1, col + 1, colorIndex);
            }
        }

        // Kalan alanları rastgele bloklarla doldur
        for (int row = 0; row < M; row++)
        {
            for (int col = 0; col < N; col++)
            {
                if (grid[row, col] == null) // Eğer bu hücreye bir blok yerleştirilmediyse
                {
                    int randomColorIndex = Random.Range(0, blockPrefabs.Length);
                    SpawnBlockWithGuaranteedColor(row, col, randomColorIndex);
                }
            }
        }
    }

    // Bir hücreye garanti renkli blok yerleştirme
    void SpawnBlockWithGuaranteedColor(int row, int col, int colorIndex)
    {
        // Kontrol ekleyerek, row ve col geçerli mi diye bakıyoruz
        if (row >= M || col >= N) return; // Eğer sınır dışıysa çık

        Vector2 spawnPosition = new Vector2(col, -row);  // Başlangıç pozisyonu
        GameObject block = Instantiate(blockPrefabs[colorIndex], spawnPosition, Quaternion.identity, transform);
        grid[row, col] = block;

        var blockController = block.GetComponent<BlockController>();
        blockController.row = row;
        blockController.col = col;
        blockController.colorIndex = colorIndex;  // Renk belirle
    }

    // GetConnectedBlocks fonksiyonu, bağlantılı blokları bulur
    private List<Vector2Int> GetConnectedBlocks(int row, int col, int colorIndex, HashSet<Vector2Int> visited)
    {
        List<Vector2Int> connectedBlocks = new List<Vector2Int>();
        Queue<Vector2Int> toVisit = new Queue<Vector2Int>();
        toVisit.Enqueue(new Vector2Int(row, col));

        while (toVisit.Count > 0)
        {
            var current = toVisit.Dequeue();
            if (visited.Contains(current)) continue;

            visited.Add(current);
            connectedBlocks.Add(current);

            var neighbors = GetNeighbors(current.x, current.y, colorIndex);
            foreach (var neighbor in neighbors)
            {
                if (!visited.Contains(neighbor))
                {
                    toVisit.Enqueue(neighbor);
                }
            }
        }

        return connectedBlocks;
    }

    // GetGrid fonksiyonu, mevcut grid'i döndürür
    public GameObject[,] GetGrid()
    {
        return grid;
    }

    // GetNeighbors fonksiyonu, bir hücrenin komşularını bulur
    private List<Vector2Int> GetNeighbors(int row, int col, int colorIndex)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();
        foreach (var dir in new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
        {
            int neighborRow = row + dir.x;
            int neighborCol = col + dir.y;
            if (neighborRow >= 0 && neighborRow < M && neighborCol >= 0 && neighborCol < N &&
                grid[neighborRow, neighborCol] != null &&
                grid[neighborRow, neighborCol].GetComponent<BlockController>().colorIndex == colorIndex)
            {
                neighbors.Add(new Vector2Int(neighborRow, neighborCol));
            }
        }
        return neighbors;

    }

    // InitializeGrid fonksiyonu, grid'i başlatır
    void InitializeGrid()
    {
        grid = new GameObject[M, N];

        for (int row = 0; row < M; row++)
        {
            for (int col = 0; col < N; col++)
            {
                SpawnBlock(row, col);
                ButtonFor00();
            }
        }
    }

    // IsPartOfGroup fonksiyonu, bir hücrenin grup içinde olup olmadığını kontrol eder
    bool IsPartOfGroup(int row, int col)
    {
        var blockController = grid[row, col]?.GetComponent<BlockController>();
        if (blockController == null) return false;

        int colorIndex = blockController.colorIndex;
        List<Vector2Int> neighbors = GetNeighbors(row, col, colorIndex);
        return neighbors.Count >= 2;
    }
    // ShuffleBoard fonksiyonu, tüm blokları karıştırır
    public void ShuffleBoard()
    {
        List<GameObject> allBlocks = new List<GameObject>();

        for (int row = 0; row < M; row++)
        {
            for (int col = 0; col < N; col++)
            {
                if (grid[row, col] != null)
                {
                    allBlocks.Add(grid[row, col]);
                    grid[row, col] = null;
                }
            }
        }

        allBlocks = allBlocks.OrderBy(x => Random.value).ToList();
        int index = 0;

        for (int row = 0; row < M; row++)
        {
            for (int col = 0; col < N; col++)
            {
                if (index < allBlocks.Count)
                {
                    grid[row, col] = allBlocks[index];
                    grid[row, col].transform.position = new Vector2(col, -row);
                    var blockController = grid[row, col].GetComponent<BlockController>();
                    blockController.row = row;
                    blockController.col = col;

                    blockController.UpdateIcon(minumumGroupSize);
                    index++;
                }
            }
        }
        ButtonFor00();
    }

    // ShuffleBoard fonksiyonu, tüm blokları karıştırır
    // public void ShuffleBoard()
    // {
    //     List<GameObject> allBlocks = new List<GameObject>();

    //     for (int row = 0; row < M; row++)
    //     {
    //         for (int col = 0; col < N; col++)
    //         {
    //             if (grid[row, col] != null)
    //             {
    //                 allBlocks.Add(grid[row, col]);
    //                 grid[row, col] = null;
    //             }
    //         }
    //     }

    //     allBlocks = allBlocks.OrderBy(x => Random.value).ToList();
    //     int index = 0;

    //     for (int row = 0; row < M; row++)
    //     {
    //         for (int col = 0; col < N; col++)
    //         {
    //             if (index < allBlocks.Count)
    //             {
    //                 grid[row, col] = allBlocks[index];
    //                 grid[row, col].transform.position = new Vector2(col, -row);
    //                 var blockController = grid[row, col].GetComponent<BlockController>();
    //                 blockController.row = row;
    //                 blockController.col = col;

    //                 blockController.UpdateIcon(minumumGroupSize);
    //                 index++;
    //             }
    //         }
    //     }
    //     if (CheckForDeadlock())
    //     {
    //         ResetGrid();

    //     }
    //     ButtonFor00();
    // }

    // SpawnBlock fonksiyonu, yeni bir blok oluşturur
    void SpawnBlock(int row, int col)
    {
        int randomColorIndex = Random.Range(0, blockPrefabs.Length);
        Vector2 spawnPosition = new Vector2(col, -row);
        GameObject block = Instantiate(blockPrefabs[randomColorIndex], spawnPosition, Quaternion.identity, transform);
        grid[row, col] = block;

        var blockController = block.GetComponent<BlockController>();
        blockController.row = row;
        blockController.col = col;
        ButtonFor00();
    }
}
