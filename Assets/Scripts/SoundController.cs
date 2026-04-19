using Unity.VisualScripting;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    [SerializeField] private AudioSource digSound;
    [SerializeField] private AudioSource winSound;
    [SerializeField] private AudioSource jingle;
    [SerializeField] private AudioSource hit;

    void Awake()
    {
        jingle.loop = true;
        if (jingle != null)
            jingle.Play();
    }

    private void OnEnable()
    {
        PickAxe.DigSound += PlayDigSound;
        PickAxe.HitSound += PlayHitSound;
        GridManager.Win += PlayWinSound;
    }

    private void OnDisable()
    {
        PickAxe.DigSound -= PlayDigSound;
        PickAxe.HitSound -= PlayHitSound;
        GridManager.Win -= PlayWinSound;
    }

    private void PlayHitSound()
    {
        if (hit != null)
            hit.Play();
    }

    private void PlayDigSound()
    {
        if (digSound != null)
            digSound.Play();
    }

    private void PlayWinSound()
    {
        if (winSound != null)
            winSound.Play();
    }
}