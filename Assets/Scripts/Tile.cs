using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector2Int GridPosition;
    public TileType Type;

    public void Init(Vector2Int gridPosition, TileType type)
    {
        GridPosition = gridPosition;
        Type = type;
    }
}