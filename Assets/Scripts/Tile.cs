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

        if (Type == TileType.Empty)
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr == null) return;

            sr.color = Color.gray;
        }

        isWalkable = type == TileType.Empty || type == TileType.Key || type == TileType.Goal;
        isReachable = false;
    }

    public void Highlight()
    {
        if (Type == TileType.Empty)
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr == null) return;

            sr.color = Color.white;
        }
    }

    public void ResetHighlight()
    {
        if (Type == TileType.Empty)
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr == null) return;

            sr.color = Color.gray;
        }
    }
}