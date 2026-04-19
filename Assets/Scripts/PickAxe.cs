using System.Collections;
using System;
using UnityEngine;

public class PickAxe : MonoBehaviour
{
    public static event Action DigSound;
    public static event Action HitSound;

    void Awake()
    {
        gameObject.SetActive(false);
    }

    public void Dig(int rotation, float duration = 0.5f)
    {
        gameObject.SetActive(true);
        if (rotation == 1)      transform.localPosition = new Vector3(-1f, 0, 0); 
        else if (rotation == 2) transform.localPosition = new Vector3(1f, 0, 0); 
        else if (rotation == 3) transform.localPosition = new Vector3(0, -1f, 0); 
        else                    transform.localPosition = new Vector3(0, 1f, 0);
        StartCoroutine(DigCoroutine(rotation, duration));
    }

    public void StopDig()
    {
        gameObject.SetActive(false);
        transform.localRotation = Quaternion.identity;
    }

    private IEnumerator DigCoroutine(int rotation, float duration)
    {
        float elapsed = 0f;

        float startAngle;
        float endAngle;

        if (rotation == 1)
        {
            startAngle = 15f;
            endAngle = 55f;
        }
        else
        {
            startAngle = -15f;
            endAngle = -55f;
        }

        int hits = 5;
        bool hitTriggered = false;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / duration;
            float swing = Mathf.PingPong(t * hits * 2f, 1f);

            float angle = Mathf.Lerp(startAngle, endAngle, swing);
            transform.localRotation = Quaternion.Euler(0f, 0f, angle);

            // keď sa dostaneš na vrchol úderu
            if (swing >= 0.95f && !hitTriggered)
            {
                HitSound?.Invoke();
                hitTriggered = true;
            }

            // reset pre ďalší hit, keď sa krompáč vráti späť
            if (swing <= 0.1f)
            {
                hitTriggered = false;
            }

            yield return null;
        }

        DigSound?.Invoke();

        transform.localRotation = Quaternion.Euler(0f, 0f, startAngle);
    }
}