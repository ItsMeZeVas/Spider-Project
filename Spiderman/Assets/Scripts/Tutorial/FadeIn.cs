using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeIn : MonoBehaviour
{
    [Header("Fade")]
    public Image fadeImage;
    public float duration = 2f;

    [Header("Audio")]
    public AudioSource audioSource;
    public float delayAfterFade = 2f;

    void Start()
    {
        StartCoroutine(FadeSequence());
    }

    IEnumerator FadeSequence()
    {
        // Fade in (de negro a transparente)
        float time = 0f;
        Color color = fadeImage.color;

        while (time < duration)
        {
            time += Time.deltaTime;
            float alpha = 1f - (time / duration);
            fadeImage.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        fadeImage.color = new Color(color.r, color.g, color.b, 0f);

        // Esperar después del fade
        yield return new WaitForSeconds(delayAfterFade);

        // Reproducir audio
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }
}