using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector2Int GridPosition;
    public TileType Type;
    public bool isWalkable;
    public bool isReachable;
    public bool locked;
    public Sprite OpenGate;

    public void Init(Vector2Int gridPosition, TileType type)
    {
        GridPosition = gridPosition;
        Type = type;

        isWalkable = type == TileType.Empty || type == TileType.Key || type == TileType.Goal;
        locked = type == TileType.Gate;
        isReachable = false;
    }

    public void Update()
    {
        if (!locked && Type == TileType.Gate)
        {
            GetComponent<SpriteRenderer>().sprite = OpenGate;
            isWalkable = true;
            locked = false;
        }
    }
}