using UnityEngine;

public class ClickToMove : MonoBehaviour
{
    public Camera cam;
    public GridManager gridManager;

    void Update()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        Vector3 mouse = Input.mousePosition;
        mouse.z = -cam.transform.position.z;

        Vector3 worldPos = cam.ScreenToWorldPoint(mouse);
        Vector2 worldPoint = new Vector2(worldPos.x, worldPos.y);

        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
        if (hit.collider == null)
            return;

        Tile tile = hit.collider.GetComponent<Tile>();
        if (tile == null)
            return;

        if (tile.Type == TileType.Diggable)
        {
            gridManager.TryMoveToDiggable(tile.GridPosition);
        }
        else
        {
            gridManager.TryMoveTo(tile.GridPosition);
        }
    }
}