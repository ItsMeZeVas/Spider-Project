using UnityEngine;
using System.Collections;

public class LabLightEndSequence : MonoBehaviour
{
    [Header("Timer")]
    public GameTimer gameTimer;

    [Header("Luces del laboratorio")]
    public Light[] labLights;

    [Header("Luz de la pantalla/tablet")]
    public Light screenLight;

    [Header("Parpadeo antes de terminar")]
    public float warningStartsAtSeconds = 10f;
    public float minFlickerDelay = 0.08f;
    public float maxFlickerDelay = 0.30f;
    public float lowIntensityMultiplier = 0.25f;

    [Header("Audio de falla eléctrica")]
    public AudioSource flickerAudioSource;
    public AudioClip electricFailureClip;
    public float minTimeBetweenFlickerSounds = 0.7f;
    [Range(0f, 1f)] public float flickerSoundChance = 0.45f;

    [Header("Transición final suave")]
    public float delayBeforeFinalFade = 0.2f;
    public float labLightsFadeOutDuration = 1.8f;
    public float screenLightFadeInDuration = 1.5f;
    public float screenLightFinalIntensity = 2f;

    [Header("Opcional")]
    public bool disableLabLightsAtEnd = true;

    private float[] originalIntensities;
    private bool flickerStarted = false;
    private bool endSequenceStarted = false;
    private Coroutine flickerCoroutine;
    private float lastFlickerSoundTime = -999f;

    void Start()
    {
        originalIntensities = new float[labLights.Length];

        for (int i = 0; i < labLights.Length; i++)
        {
            if (labLights[i] != null)
                originalIntensities[i] = labLights[i].intensity;
        }

        if (screenLight != null)
        {
            screenLight.enabled = true;
            screenLight.intensity = 0f;
        }

        if (gameTimer != null)
            gameTimer.OnGameEnded += HandleGameEnded;
        else
            Debug.LogWarning("LabLightEndSequence: falta asignar GameTimer.");
    }

    void OnDestroy()
    {
        if (gameTimer != null)
            gameTimer.OnGameEnded -= HandleGameEnded;
    }

    void Update()
    {
        if (gameTimer == null || gameTimer.GameEnded)
            return;

        if (!flickerStarted && gameTimer.TimeRemaining <= warningStartsAtSeconds)
        {
            flickerStarted = true;
            flickerCoroutine = StartCoroutine(FlickerLights());
        }
    }

    IEnumerator FlickerLights()
    {
        while (gameTimer != null && !gameTimer.GameEnded)
        {
            bool useLowIntensity = Random.value > 0.5f;

            for (int i = 0; i < labLights.Length; i++)
            {
                if (labLights[i] == null) continue;

                labLights[i].intensity = useLowIntensity
                    ? originalIntensities[i] * lowIntensityMultiplier
                    : originalIntensities[i];
            }

            TryPlayFlickerSound();

            yield return new WaitForSeconds(Random.Range(minFlickerDelay, maxFlickerDelay));
        }
    }

    void TryPlayFlickerSound()
    {
        if (flickerAudioSource == null || electricFailureClip == null)
            return;

        if (Time.time < lastFlickerSoundTime + minTimeBetweenFlickerSounds)
            return;

        if (Random.value > flickerSoundChance)
            return;

        flickerAudioSource.pitch = Random.Range(0.92f, 1.08f);
        flickerAudioSource.PlayOneShot(electricFailureClip);

        lastFlickerSoundTime = Time.time;
    }

    void HandleGameEnded()
    {
        if (endSequenceStarted)
            return;

        endSequenceStarted = true;

        if (flickerCoroutine != null)
            StopCoroutine(flickerCoroutine);

        StartCoroutine(SmoothFinalLightsSequence());
    }

    IEnumerator SmoothFinalLightsSequence()
    {
        yield return new WaitForSeconds(delayBeforeFinalFade);

        float[] currentLabIntensities = new float[labLights.Length];

        for (int i = 0; i < labLights.Length; i++)
        {
            if (labLights[i] != null)
                currentLabIntensities[i] = labLights[i].intensity;
        }

        if (screenLight != null)
        {
            screenLight.enabled = true;
            screenLight.intensity = 0f;
        }

        float totalDuration = Mathf.Max(labLightsFadeOutDuration, screenLightFadeInDuration);
        float time = 0f;

        while (time < totalDuration)
        {
            time += Time.deltaTime;

            float labT = Mathf.Clamp01(time / labLightsFadeOutDuration);
            float screenT = Mathf.Clamp01(time / screenLightFadeInDuration);

            labT = Mathf.SmoothStep(0f, 1f, labT);
            screenT = Mathf.SmoothStep(0f, 1f, screenT);

            for (int i = 0; i < labLights.Length; i++)
            {
                if (labLights[i] == null) continue;

                labLights[i].intensity = Mathf.Lerp(
                    currentLabIntensities[i],
                    0f,
                    labT
                );
            }

            if (screenLight != null)
            {
                screenLight.intensity = Mathf.Lerp(
                    0f,
                    screenLightFinalIntensity,
                    screenT
                );
            }

            yield return null;
        }

        for (int i = 0; i < labLights.Length; i++)
        {
            if (labLights[i] == null) continue;

            labLights[i].intensity = 0f;

            if (disableLabLightsAtEnd)
                labLights[i].enabled = false;
        }

        if (screenLight != null)
        {
            screenLight.enabled = true;
            screenLight.intensity = screenLightFinalIntensity;
        }
    }
}