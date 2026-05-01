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

        PlayInstruction(shootInstruction);

        yield return new WaitUntil(() => shotDone);

        if (waitForTargetHit)
        {
            yield return new WaitUntil(() => targetHitDone);
        }

        yield return new WaitForSeconds(1f);

        PlayInstruction(reloadInstruction);

        yield return new WaitUntil(() => reloadDone);

        yield return new WaitForSeconds(1f);

        PlayInstruction(endTutorial);

        yield return new WaitForSeconds(delayBeforeChangingScene);

        GoToLevelScene();
    }

    void PlayInstruction(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    void GoToLevelScene()
    {
        if (fadeTransition != null)
        {
            fadeTransition.LoadSceneWithFade(levelSceneName);
        }
        else
        {
            Debug.LogWarning("No hay FadeTransition asignado. Cargando LevelScene directamente.");
            SceneManager.LoadScene(levelSceneName);
        }
    }
}