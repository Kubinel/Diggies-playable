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
}