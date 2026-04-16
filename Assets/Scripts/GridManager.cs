using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Map Data")]
    public List<string> mapData = new List<string>();

    [Header("Prefabs")]
    public GameObject floorPrefab;
    public GameObject wallPrefab;
    public GameObject diggablePrefab;
    public GameObject keyPrefab;
    public GameObject gatePrefab;
    public GameObject goalPrefab;
    public GameObject playerPrefab;

    [Header("Settings")]
    public float tileSize = 1f;
    public Transform mapRoot;

    private TileType[,] grid;
    private GameObject[,] spawnedObjects;

    public Vector2Int PlayerStartPos { get; private set; }

    public int Width;
    public int Height;

    void Awake()
    {
        Map map = new Map();
        mapData = map.mapData;
        Width = mapData[0].Length;
        Height = mapData.Count;

        DrawMap();
        RecalculateReachable();
    }

    public void DrawMap()
    {
        grid = new TileType[Width, Height];
        spawnedObjects = new GameObject[Width, Height];

        for (int row = 0; row < Height; row++)
        {
            string line = mapData[row];

            for (int col = 0; col < Width; col++)
            {
                char c = line[col];

                int x = col;
                int y = Height - 1 - row;

                Vector2Int gridPos = new Vector2Int(x, y);
                Vector3 worldPos = GridToWorld(gridPos);

                switch (c)
                {
                    case '#':
                        SpawnTile(wallPrefab, gridPos, worldPos, TileType.Wall);
                        break;
                    case '.':
                        SpawnTile(floorPrefab, gridPos, worldPos, TileType.Empty);
                        break;
                    case 'D':
                        SpawnTile(diggablePrefab, gridPos, worldPos, TileType.Diggable);
                        break;
                    case 'K':
                        SpawnTile(keyPrefab, gridPos, worldPos, TileType.Key);
                        break;
                    case 'B':
                        SpawnTile(gatePrefab, gridPos, worldPos, TileType.Gate);
                        break;
                    case 'G':
                        SpawnTile(goalPrefab, gridPos, worldPos, TileType.Goal);
                        break;
                    case 'P':
                        PlayerStartPos = gridPos;
                        SpawnTile(floorPrefab, gridPos, worldPos, TileType.Empty);
                        Instantiate(playerPrefab, worldPos, Quaternion.identity);
                        break;
                    default:
                        Debug.LogWarning($"Unknown map char '{c}' at row {row}, col {col}");
                        SpawnTile(floorPrefab, gridPos, worldPos, TileType.Empty);
                        break;
                }
            }
        }
    }

    private void SpawnTile(GameObject prefab, Vector2Int gridPos, Vector3 worldPos, TileType type)
    {
        GameObject obj = Instantiate(prefab, worldPos, Quaternion.identity, mapRoot);
        Tile tile = obj.GetComponent<Tile>();

        if (tile != null)
            tile.Init(gridPos, type);

        grid[gridPos.x, gridPos.y] = type;
        spawnedObjects[gridPos.x, gridPos.y] = obj;
    }

    public Vector3 GridToWorld(Vector2Int gridPos)
    {
        float mapWidth = Width * tileSize;
        float mapHeight = Height * tileSize;

        float originX = -mapWidth / 2f + tileSize / 2f;
        float originY = -mapHeight / 2f + tileSize / 2f;

        return new Vector3(
            originX + gridPos.x * tileSize,
            originY + gridPos.y * tileSize,
            0f
        );
    }

    public bool IsInside(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < Width && pos.y >= 0 && pos.y < Height;
    }

    public Tile GetTile(Vector2Int pos)
    {
        if (!IsInside(pos)) return null;

        GameObject obj = spawnedObjects[pos.x, pos.y];
        if (obj == null) return null;

        return obj.GetComponent<Tile>();
    }

    public void RecalculateReachable()
    {
        // 1. reset
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Tile tile = GetTile(new Vector2Int(x, y));
                if (tile != null)
                {
                    tile.isReachable = false;
                    tile.ResetHighlight();
                }
            }
        }

        // 2. start tile
        Tile startTile = GetTile(PlayerStartPos);
        if (startTile == null)
        {
            Debug.LogWarning("PlayerStartPos tile not found.");
            return;
        }

        if (!startTile.isWalkable)
        {
            Debug.LogWarning("Player start tile is not walkable.");
            return;
        }

        // 3. BFS
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(PlayerStartPos);
        startTile.isReachable = true;

        Vector2Int[] directions = new Vector2Int[]
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            foreach (Vector2Int dir in directions)
            {
                Vector2Int next = current + dir;

                if (!IsInside(next))
                    continue;

                Tile nextTile = GetTile(next);
                if (nextTile == null)
                    continue;

                if (!nextTile.isWalkable)
                    continue;

                if (nextTile.isReachable)
                    continue;

                nextTile.isReachable = true;
                if (nextTile.Type == TileType.Empty)
                {
                    nextTile.Highlight();
                }
                queue.Enqueue(next);
            }
        }
    }
}