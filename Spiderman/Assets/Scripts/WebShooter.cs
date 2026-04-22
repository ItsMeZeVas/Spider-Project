using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class WebShooter : MonoBehaviour
{
    [Header("Configuración")]
    public GameObject webProjectilePrefab;
    public Transform shootPoint;
    public float shootForce = 20f;

    [Header("Munición")]
    public int maxAmmo = 7;

    [Header("Input")]
    [Tooltip("XRI Right/Activate/Value")]
    public InputActionReference triggerAction;

    [Tooltip("XRI Right/Select/Value (Grip)")]
    public InputActionReference gripAction;

    [System.Serializable]
    public class AudioVariation
    {
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
    }

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioVariation[] shootSounds;
    public AudioVariation[] reloadSounds;
    public AudioVariation[] emptySounds;

    [Header("Pitch aleatorio")]
    [Range(0.1f, 1.2f)] public float minPitch = 0.9f;
    [Range(0.1f, 1.2f)] public float maxPitch = 1.1f;

    private int currentAmmo;
    private bool isReloading;
    private bool triggerWasPressed;
    private bool gripWasPressed;

    void Awake()
    {
        currentAmmo = maxAmmo;
    }

    void OnEnable()
    {
        triggerAction?.action.Enable();
        gripAction?.action.Enable();
    }

    void OnDisable()
    {
        triggerAction?.action.Disable();
        gripAction?.action.Disable();
    }

    void Update()
    {
        HandleTrigger();
        HandleGrip();
    }

    void HandleTrigger()
    {
        if (triggerAction == null) return;

        bool triggerPressed = triggerAction.action.ReadValue<float>() > 0.5f;

        if (triggerPressed && !triggerWasPressed)
        {
            if (currentAmmo > 0 && !isReloading)
            {
                Shoot();
            }
            else
            {
                Debug.Log("Sin telarañas — usa recarga");
                PlayRandomSound(emptySounds);
            }
        }

        triggerWasPressed = triggerPressed;
    }

    void HandleGrip()
    {
        if (gripAction == null) return;

        bool gripPressed = gripAction.action.ReadValue<float>() > 0.5f;

        if (gripPressed && !gripWasPressed && !isReloading)
        {
            StartCoroutine(Reload());
        }

        gripWasPressed = gripPressed;
    }

    void Shoot()
    {
        if (webProjectilePrefab == null || shootPoint == null) return;

        currentAmmo--;
        Debug.Log($"Telarañas restantes: {currentAmmo}/{maxAmmo}");
        PlayRandomSound(shootSounds);

        GameObject projectile = Instantiate(webProjectilePrefab, shootPoint.position, shootPoint.rotation);

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(shootPoint.forward * shootForce, ForceMode.VelocityChange);
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Recargando...");
        PlayRandomSound(reloadSounds);

        yield return new WaitForSeconds(1.5f);

        currentAmmo = maxAmmo;
        isReloading = false;
        Debug.Log($"¡Recargado! {currentAmmo}/{maxAmmo}");
    }

    // Disparo desde gestos
    public void ActivateFromGesture()
    {
        if (currentAmmo > 0 && !isReloading)
        {
            Shoot();
        }
        else
        {
            Debug.Log("Sin telarañas — recarga con el puño");
            PlayRandomSound(emptySounds);
        }
    }

    // Recarga desde gestos
    public void ActivateReloadFromGesture()
    {
        if (!isReloading && currentAmmo < maxAmmo)
        {
            StartCoroutine(Reload());
        }
        else if (!isReloading && currentAmmo >= maxAmmo)
        {
            Debug.Log("La munición ya está completa");
        }
    }

    void PlayRandomSound(AudioVariation[] sounds)
    {
        if (audioSource == null || sounds == null || sounds.Length == 0) return;

        AudioVariation selected = sounds[Random.Range(0, sounds.Length)];
        if (selected.clip == null) return;

        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.PlayOneShot(selected.clip, selected.volume);
    }
}