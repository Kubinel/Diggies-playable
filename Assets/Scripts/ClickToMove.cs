using Unity.VisualScripting;
using UnityEngine;

public class ClickToMove : MonoBehaviour
{
    public Camera cam;
    public GridManager gridManager;

    [SerializeField] private GameObject hand;
    [SerializeField] private GameObject Ui;
    [SerializeField] private float handMoveAmount = 1f;
    [SerializeField] private float handMoveSpeed = 2f;

    private Vector3 handStartPos;
    private Transform handTransform;
    private int numberOfClicks = 0;

    void Start()
    {
        numberOfClicks = 0;
        if (hand != null)
        {
            hand.SetActive(false);
            handTransform = hand.transform;
            handStartPos = handTransform.localPosition;
        }
    }

    void Update()
    {
        if (hand != null && numberOfClicks == 1)
        {
            hand.SetActive(true);
            float offsetY = Mathf.PingPong(Time.time * handMoveSpeed, handMoveAmount);
            handTransform.localPosition = handStartPos + new Vector3(0f, offsetY, 0f);
        }
        
        if (!Input.GetMouseButtonDown(0))
            return;

        if (Ui != null && numberOfClicks == 0)
        {
            Ui.SetActive(false);
        }

        numberOfClicks++;
        if (hand != null && numberOfClicks == 2) hand.SetActive(false);

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