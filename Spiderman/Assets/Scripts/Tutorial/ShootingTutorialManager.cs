using UnityEngine;
using System.Collections;

public class ShootingTutorialManager : MonoBehaviour
{
    [Header("Shooters")]
    public WebShooterTutorial[] shooters; // 👈 ahora son varios

    [Header("Audios")]
    public AudioSource audioSource;
    public AudioClip shootInstruction;
    public AudioClip reloadInstruction;
    public AudioClip endTutorial;

    private bool shotDone = false;
    private bool reloadDone = false;

    void Start()
    {
        // Suscribirse a TODOS los shooters
        foreach (var shooter in shooters)
        {
            shooter.OnFirstShoot += HandleFirstShoot;
            shooter.OnFirstReload += HandleFirstReload;
        }

        StartCoroutine(TutorialFlow());
    }

    void HandleFirstShoot()
    {
        shotDone = true;
    }

    void HandleFirstReload()
    {
        reloadDone = true;
    }

    IEnumerator TutorialFlow()
    {
        yield return new WaitForSeconds(1f);

        // 🔊 "Dispara"
        audioSource.PlayOneShot(shootInstruction);

        yield return new WaitUntil(() => shotDone);

        yield return new WaitForSeconds(1f);

        // 🔊 "Recarga"
        audioSource.PlayOneShot(reloadInstruction);

        yield return new WaitUntil(() => reloadDone);

        yield return new WaitForSeconds(1f);

        // 🔊 Final
        audioSource.PlayOneShot(endTutorial);
    }
}