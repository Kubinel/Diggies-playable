using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ResponsiveCamera : MonoBehaviour
{
    public GridManager gridManager;
    public float padding = 0.5f;

    private Camera cam;
    private int lastWidth;
    private int lastHeight;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void Start()
    {
        ApplyFit();
        lastWidth = Screen.width;
        lastHeight = Screen.height;
    }

    void Update()
    {
        if (Screen.width != lastWidth || Screen.height != lastHeight)
        {
            lastWidth = Screen.width;
            lastHeight = Screen.height;
            ApplyFit();
        }
    }

    public void ApplyFit()
    {
        if (gridManager == null)
        {
            Debug.LogWarning("ResponsiveCamera: gridManager is not assigned.");
            return;
        }

        if (gridManager.Width <= 0 || gridManager.Height <= 0)
        {
            Debug.LogWarning("ResponsiveCamera: grid size is not ready yet.");
            return;
        }

        float mapWidth = gridManager.Width * gridManager.tileSize;
        float mapHeight = gridManager.Height * gridManager.tileSize;

        float screenAspect = (float)Screen.width / Screen.height;
        float targetAspect = mapWidth / mapHeight;

        if (screenAspect >= targetAspect)
        {
            cam.orthographicSize = mapHeight / 2f + padding;
        }
        else
        {
            cam.orthographicSize = (mapWidth / screenAspect) / 2f + padding;
        }

        cam.transform.position = new Vector3(0f, 0f, -10f);
    }
}