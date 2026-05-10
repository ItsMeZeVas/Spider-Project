using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ShootingTutorialManager : MonoBehaviour
{
    [Header("Shooters del tutorial")]
    public WebShooterTutorial[] shooters;

    [Header("Audios del tutorial")]
    public AudioSource audioSource;

    [Tooltip("Primer audio: bienvenida e indicación de disparo")]
    public AudioClip shootInstruction;

    [Tooltip("Segundo audio: indicación de cómo recargar")]
    public AudioClip reloadInstruction;

    [Tooltip("Tercer audio: 'lo hiciste muy bien, ya estás listo para empezar'")]
    public AudioClip goodJobFeedback;

    [Tooltip("Cuarto audio: 'ahora nadie podrá detenerme'")]
    public AudioClip characterLine;

    [Header("Transición")]
    public SceneFadeTransition fadeTransition;
    public string levelSceneName = "LevelScene";
    public float delayBeforeChangingScene = 2f;

    private bool shotDone = false;
    private bool reloadDone = false;

    void OnDisable()
    {
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

    IEnumerator TutorialFlow()
    {
        yield return new WaitForSeconds(1f);

        SetShootPermission(false);
        SetReloadPermission(false);

        yield return StartCoroutine(PlayInstructionAndWait(shootInstruction));

        SetShootPermission(true);
        SetReloadPermission(false);

        yield return new WaitUntil(() => shotDone);


        SetShootPermission(true);
        SetReloadPermission(false);

        yield return StartCoroutine(PlayInstructionAndWait(reloadInstruction));

        SetShootPermission(true);
        SetReloadPermission(true);

        yield return new WaitUntil(() => reloadDone);


        yield return StartCoroutine(PlayInstructionAndWait(goodJobFeedback));

        yield return StartCoroutine(PlayInstructionAndWait(characterLine));

        yield return new WaitForSeconds(delayBeforeChangingScene);

        GoToLevelScene();
    }

    IEnumerator PlayInstructionAndWait(AudioClip clip)
    {
        if (audioSource == null || clip == null)
            yield break;

        audioSource.PlayOneShot(clip);
        yield return new WaitForSeconds(clip.length);
    }

    void SetShootPermission(bool value)
    {
        if (shooters == null) return;

        foreach (var shooter in shooters)
        {
            if (shooter == null) continue;
            shooter.SetCanShoot(value);
        }
    }

    void SetReloadPermission(bool value)
    {
        if (shooters == null) return;

        foreach (var shooter in shooters)
        {
            if (shooter == null) continue;
            shooter.SetCanReload(value);
        }
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