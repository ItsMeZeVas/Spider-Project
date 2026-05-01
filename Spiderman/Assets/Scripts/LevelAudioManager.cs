using UnityEngine;
using System.Collections;

public class LevelAudioManager : MonoBehaviour
{
    [Header("Timer")]
    public GameTimer gameTimer;

    [Header("Audio Source")]
    public AudioSource audioSource;

    [Header("Clips")]
    public AudioClip enemiesDetectedClip;
    public AudioClip powerDownClip;

    [Header("Configuración")]
    public float startAudioDelay = 0.5f;

    void Start()
    {
        if (gameTimer != null)
            gameTimer.OnGameEnded += HandleGameEnded;
        else
            Debug.LogWarning("LevelAudioManager: falta asignar GameTimer.");

        StartCoroutine(PlayStartAudio());
    }

    void OnDestroy()
    {
        if (gameTimer != null)
            gameTimer.OnGameEnded -= HandleGameEnded;
    }

    IEnumerator PlayStartAudio()
    {
        yield return new WaitForSeconds(startAudioDelay);

        if (audioSource != null && enemiesDetectedClip != null)
            audioSource.PlayOneShot(enemiesDetectedClip);
    }

    void HandleGameEnded()
    {
        if (audioSource != null && powerDownClip != null)
            audioSource.PlayOneShot(powerDownClip);
    }
}