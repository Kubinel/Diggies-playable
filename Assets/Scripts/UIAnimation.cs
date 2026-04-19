using System.Collections;
using UnityEngine;

public class UIAnimation : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform flyingObject;
    [SerializeField] private CanvasGroup fadingObject;

    [Header("Positions")]
    [SerializeField] private Vector2 startPos = new Vector2(-800f, 0f);
    [SerializeField] private Vector2 targetPos = new Vector2(0f, 0f);

    [Header("Timing")]
    [SerializeField] private float flyDuration = 0.8f;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float spinOutDuration = 0.35f;

    private void OnEnable()
    {
        GridManager.Win += aaaa;
    }

    private void OnDisable()
    {
        GridManager.Win -= aaaa;
    }

    private void aaaa()
    {
        StartCoroutine(PlayIntro());
    }

    private IEnumerator PlayIntro()
    {
        // Initial state
        flyingObject.anchoredPosition = startPos;
        flyingObject.localRotation = Quaternion.identity;

        fadingObject.alpha = 0f;
        fadingObject.interactable = false;
        fadingObject.blocksRaycasts = false;

        // 1. Fly in
        float elapsed = 0f;
        while (elapsed < flyDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / flyDuration);

            // smooth-ish movement
            float eased = Mathf.SmoothStep(0f, 1f, t);
            flyingObject.anchoredPosition = Vector2.Lerp(startPos, targetPos, eased);

            yield return null;
        }

        flyingObject.anchoredPosition = targetPos;

        // 2. Fade in second object
        elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);

            fadingObject.alpha = t;
            yield return null;
        }

        fadingObject.alpha = 1f;
        fadingObject.interactable = true;
        fadingObject.blocksRaycasts = true;

        // 3. Rotate in place and disappear
        elapsed = 0f;
        while (elapsed < spinOutDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / spinOutDuration);

            flyingObject.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(0f, 180f, t));

            // optional scale-down while disappearing
            float scale = Mathf.Lerp(1f, 0f, t);
            flyingObject.localScale = new Vector3(scale, scale, 1f);

            yield return null;
        }

        flyingObject.gameObject.SetActive(false);
    }
}