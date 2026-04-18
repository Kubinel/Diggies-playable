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
    public PlayerController Player { get; private set; }
    public Vector2Int CurrentPlayerPos { get; private set; }

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
                        SpawnTile(floorPrefab, gridPos, worldPos, TileType.Empty);
                        SpawnTile(diggablePrefab, gridPos, worldPos, TileType.Diggable);
                        break;
                    case 'K':
                        SpawnTile(floorPrefab, gridPos, worldPos, TileType.Empty);
                        SpawnTile(keyPrefab, gridPos, worldPos, TileType.Key);
                        break;
                    case 'B':
                        SpawnTile(floorPrefab, gridPos, worldPos, TileType.Empty);
                        SpawnTile(gatePrefab, gridPos, worldPos, TileType.Gate);
                        break;
                    case 'G':
                        SpawnTile(floorPrefab, gridPos, worldPos, TileType.Empty);
                        SpawnTile(goalPrefab, gridPos, worldPos, TileType.Goal);
                        break;
                    case 'P':
                        PlayerStartPos = gridPos;
                        CurrentPlayerPos = gridPos;
                        SpawnTile(floorPrefab, gridPos, worldPos, TileType.Empty);

                        GameObject playerObj = Instantiate(playerPrefab, worldPos, Quaternion.identity);
                        Player = playerObj.GetComponent<PlayerController>();

                        if (Player != null)
                        {
                            Player.Init(this, gridPos);
                        }
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
                }
            }
        }

        // 2. start tile
        Tile startTile = GetTile(CurrentPlayerPos);
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
        queue.Enqueue(CurrentPlayerPos);
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
                queue.Enqueue(next);
            }
        }
    }

    public List<Vector2Int> FindPath(Vector2Int start, Vector2Int target)
    {
        if (!IsInside(start) || !IsInside(target))
            return null;

        Tile startTile = GetTile(start);
        Tile targetTile = GetTile(target);

        if (startTile == null || targetTile == null)
            return null;

        if (!targetTile.isWalkable)
            return null;

        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        queue.Enqueue(start);
        visited.Add(start);

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

            if (current == target)
            {
                return ReconstructPath(cameFrom, start, target);
            }

            foreach (Vector2Int dir in directions)
            {
                Vector2Int next = current + dir;

                if (!IsInside(next))
                    continue;

                if (visited.Contains(next))
                    continue;

                Tile nextTile = GetTile(next);
                if (nextTile == null || !nextTile.isWalkable)
                    continue;

                visited.Add(next);
                cameFrom[next] = current;
                queue.Enqueue(next);
            }
        }

        return null;
    }

    private List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int start, Vector2Int target)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int current = target;

        while (current != start)
        {
            path.Add(current);
            current = cameFrom[current];
        }

        path.Reverse();
        return path;
    }

    public void TryMoveTo(Vector2Int targetPos)
    {
        Tile targetTile = GetTile(targetPos);
        if (targetTile == null)
            return;

        if (!targetTile.isWalkable)
            return;

        if (!targetTile.isReachable)
            return;

        if (Player == null || Player.IsMoving)
            return;

        List<Vector2Int> path = FindPath(CurrentPlayerPos, targetPos);
        if (path == null || path.Count == 0)
            return;

        Player.MoveAlongPath(path, () =>
        {
            CurrentPlayerPos = targetPos;
            RecalculateReachable();
        });
    }

    public Vector2Int? GetAdjacentReachableTile(Vector2Int diggablePos)
    {
        Vector2Int[] directions = new Vector2Int[]
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        Vector2Int? bestPos = null;
        int bestDistance = int.MaxValue;

        foreach (Vector2Int dir in directions)
        {
            Vector2Int adjacent = diggablePos + dir;

            if (!IsInside(adjacent))
                continue;

            Tile tile = GetTile(adjacent);
            if (tile == null)
                continue;

            if (!tile.isWalkable)
                continue;

            if (!tile.isReachable)
                continue;

            int distance = Mathf.Abs(adjacent.x - CurrentPlayerPos.x) + Mathf.Abs(adjacent.y - CurrentPlayerPos.y);
            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestPos = adjacent;
            }
        }

        return bestPos;
    }

    public void TryMoveToDiggable(Vector2Int diggablePos)
    {
        Tile diggableTile = GetTile(diggablePos);
        if (diggableTile == null)
            return;

        if (diggableTile.Type != TileType.Diggable)
            return;

        if (Player == null || Player.IsMoving)
            return;

        Vector2Int? adjacentPos = GetAdjacentReachableTile(diggablePos);
        if (adjacentPos == null)
            return;

        List<Vector2Int> path = FindPath(CurrentPlayerPos, adjacentPos.Value);
        if (path == null)
            return;

        Player.MoveAlongPath(path, () =>
        {
            CurrentPlayerPos = adjacentPos.Value;
            RecalculateReachable();
        });
    }

}