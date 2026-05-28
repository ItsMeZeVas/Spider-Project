using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class AudioDucker : MonoBehaviour
{
    public AudioMixer mixer;
    public void LowerMusic()
    {
        StopAllCoroutines();
        StartCoroutine(FadeMusic(-30f, 0.5f));
    }

    public void RestoreMusic()
    {
        StopAllCoroutines();
        StartCoroutine(FadeMusic(0f, 0.5f));
    }

    IEnumerator FadeMusic(float targetVolume, float duration)
    {
        mixer.GetFloat("MusicVol", out float currentVolume);

        float time = 0;

        while (time < duration)
        {
            float volume = Mathf.Lerp(currentVolume, targetVolume, time / duration);

            mixer.SetFloat("MusicVol", volume);

            time += Time.deltaTime;

            yield return null;
        }

        mixer.SetFloat("MusicVol", targetVolume);
    }
}