using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector2Int GridPosition;
    public TileType Type;
    public bool isWalkable;
    public bool isReachable;

    public void Init(Vector2Int gridPosition, TileType type)
    {
        GridPosition = gridPosition;
        Type = type;

        isWalkable = type == TileType.Empty || type == TileType.Key || type == TileType.Goal;
        isReachable = false;
    }
}