using System;
using System.Collections;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector2Int GridPosition;
    public TileType Type;
    public bool isWalkable;
    public bool isReachable;
    public bool locked;
    public Sprite OpenGate;

    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float shakeStrength = 0.06f;
    [SerializeField] private float shakeSpeed = 40f;

    private Vector3 originalLocalPos;
    private bool isBreaking = false;

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

    public void BreakTile(Action onComplete = null)
    {
        if (isBreaking) return;

        if (Type == TileType.Diggable)
        {
            StartCoroutine(BreakCoroutine(onComplete));
        }
    }

    private IEnumerator BreakCoroutine(Action onComplete)
    {
        isBreaking = true;

        originalLocalPos = transform.localPosition;

        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;

            float offsetX = Mathf.Sin(elapsed * shakeSpeed) * shakeStrength;
            float offsetY = Mathf.Cos(elapsed * shakeSpeed * 1.3f) * shakeStrength * 0.35f;

            transform.localPosition = originalLocalPos + new Vector3(offsetX, offsetY, 1f);

            yield return null;
        }

        transform.localPosition = originalLocalPos;

        onComplete?.Invoke();
    }
}