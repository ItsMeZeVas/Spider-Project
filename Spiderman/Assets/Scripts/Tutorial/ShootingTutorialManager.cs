using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ShootingTutorialManager : MonoBehaviour
{
    [Header("Shooters del tutorial")]
    public WebShooterTutorial[] shooters;

    [Header("Audios del tutorial")]
    public AudioSource audioSource;
    public AudioClip shootInstruction;
    public AudioClip reloadInstruction;
    public AudioClip endTutorial;

    [Header("Objetivo de práctica")]
    public bool waitForTargetHit = true;

    [Header("Transición")]
    public SceneFadeTransition fadeTransition;
    public string levelSceneName = "LevelScene";
    public float delayBeforeChangingScene = 2f;

    private bool shotDone = false;
    private bool reloadDone = false;
    private bool targetHitDone = false;

    void OnEnable()
    {
        TutorialPracticeTarget.OnAnyPracticeTargetHit += HandleTargetHit;
    }

    void OnDisable()
    {
        TutorialPracticeTarget.OnAnyPracticeTargetHit -= HandleTargetHit;
        if (shooters != null)
        {
            foreach (var shooter in shooters)
            {
                if (shooter == null) continue;
                shooter.OnFirstShoot -= HandleFirstShoot;
                shooter.OnFirstReload -= HandleFirstReload;
            }
        }
    }

    void Start()
    {
        // Desactivar shooters al inicio
        SetShootersEnabled(false);

        if (shooters != null)
        {
            foreach (var shooter in shooters)
            {
                if (shooter == null) continue;
                shooter.OnFirstShoot += HandleFirstShoot;
                shooter.OnFirstReload += HandleFirstReload;
            }
        }

        StartCoroutine(TutorialFlow());
    }

    void HandleFirstShoot()
    {
        shotDone = true;
        Debug.Log("Tutorial: primer disparo detectado.");
    }

    void HandleFirstReload()
    {
        reloadDone = true;
        Debug.Log("Tutorial: primera recarga detectada.");
    }

    void HandleTargetHit()
    {
        targetHitDone = true;
        Debug.Log("Tutorial: diana impactada.");
    }

    IEnumerator TutorialFlow()
    {
        yield return new WaitForSeconds(1f);

        // Reproducir instrucción de disparo y esperar que termine
        yield return StartCoroutine(PlayInstructionAndWait(shootInstruction));

        // Activar shooters cuando termina el primer audio
        SetShootersEnabled(true);

        yield return new WaitUntil(() => shotDone);

        if (waitForTargetHit)
            yield return new WaitUntil(() => targetHitDone);

        yield return new WaitForSeconds(1f);

        // Desactivar shooters antes de instrucción de recarga
        SetShootersEnabled(false);

        // Reproducir instrucción de recarga y esperar que termine
        yield return StartCoroutine(PlayInstructionAndWait(reloadInstruction));

        // Activar shooters para que pueda recargar
        SetShootersEnabled(true);

        yield return new WaitUntil(() => reloadDone);

        yield return new WaitForSeconds(1f);

        // Desactivar shooters al final
        SetShootersEnabled(false);

        // Reproducir audio final y esperar que termine
        yield return StartCoroutine(PlayInstructionAndWait(endTutorial));

        yield return new WaitForSeconds(delayBeforeChangingScene);

        GoToLevelScene();
    }

    IEnumerator PlayInstructionAndWait(AudioClip clip)
    {
        if (audioSource == null || clip == null) yield break;
        audioSource.PlayOneShot(clip);
        yield return new WaitForSeconds(clip.length);
    }

    void SetShootersEnabled(bool enabled)
    {
        if (shooters == null) return;
        foreach (var shooter in shooters)
        {
            if (shooter == null) continue;
            shooter.enabled = enabled;
        }
    }

    void PlayInstruction(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }

    void GoToLevelScene()
    {
        if (fadeTransition != null)
            fadeTransition.LoadSceneWithFade(levelSceneName);
        else
        {
            Debug.LogWarning("No hay FadeTransition asignado. Cargando LevelScene directamente.");
            SceneManager.LoadScene(levelSceneName);
        }
    }
}