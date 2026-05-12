using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System;

public class WebShooterTutorial : MonoBehaviour
{
    [Header("Antispam")]
    public float gestureShotCooldown = 0.20f;
    private float lastGestureShotTime = -999f;

    [Header("Configuración")]
    public GameObject webProjectilePrefab;
    public Transform shootPoint;
    public float shootForce = 20f;

    [Header("Munición")]
    public int maxAmmo = 7;

    [Header("Input por mandos")]
    [Tooltip("XRI Right/Activate/Value o Left/Activate/Value")]
    public InputActionReference triggerAction;

    [Tooltip("XRI Right/Select/Value o Left/Select/Value")]
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

    public int currentAmmo;
    private bool isReloading;
    private bool triggerWasPressed;
    private bool gripWasPressed;

    private bool canShoot = false;
    private bool canReload = false;

    public event Action OnFirstShoot;
    public event Action OnFirstReload;

    private bool hasShot = false;
    private bool hasReloaded = false;

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
            TryShoot();
        }

        triggerWasPressed = triggerPressed;
    }

    void HandleGrip()
    {
        if (gripAction == null) return;

        bool gripPressed = gripAction.action.ReadValue<float>() > 0.5f;

        if (gripPressed && !gripWasPressed)
        {
            TryReload();
        }

        gripWasPressed = gripPressed;
    }

    public void ActivateFromGesture()
    {
        if (Time.time < lastGestureShotTime + gestureShotCooldown)
            return;

        lastGestureShotTime = Time.time;
        TryShoot();
    }

    public void ActivateReloadFromGesture()
    {
        TryReload();
    }

    public void SetCanShoot(bool value)
    {
        canShoot = value;
    }

    public void SetCanReload(bool value)
    {
        canReload = value;
    }

    void TryShoot()
    {
        if (!canShoot)
            return;

        if (isReloading)
            return;

        if (currentAmmo <= 0)
        {
            Debug.Log("Sin telarañas — recarga con Grip.");
            PlayRandomSound(emptySounds);
            return;
        }

        Shoot();
    }

    void TryReload()
    {
        if (!canReload)
            return;

        if (!isReloading && currentAmmo < maxAmmo)
        {
            if (!hasReloaded)
            {
                hasReloaded = true;
                OnFirstReload?.Invoke();
            }

            StartCoroutine(Reload());
        }
    }

    void Shoot()
    {
        if (webProjectilePrefab == null || shootPoint == null)
        {
            Debug.LogWarning($"{name}: falta Web Projectile Prefab o Shoot Point.");
            return;
        }

        currentAmmo--;

        Debug.Log($"Tutorial - Telarañas restantes: {currentAmmo}/{maxAmmo}");

        PlayRandomSound(shootSounds);

        if (!hasShot)
        {
            hasShot = true;
            OnFirstShoot?.Invoke();
        }

        GameObject projectile = Instantiate(
            webProjectilePrefab,
            shootPoint.position,
            shootPoint.rotation
        );

        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.AddForce(shootPoint.forward * shootForce, ForceMode.VelocityChange);
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;

        Debug.Log("Tutorial - Recargando...");
        PlayRandomSound(reloadSounds);

        yield return new WaitForSeconds(1.5f);

        currentAmmo = maxAmmo;
        isReloading = false;

        Debug.Log($"Tutorial - Recargado: {currentAmmo}/{maxAmmo}");
    }

    void PlayRandomSound(AudioVariation[] sounds)
    {
        if (audioSource == null || sounds == null || sounds.Length == 0)
            return;

        AudioVariation selected = sounds[UnityEngine.Random.Range(0, sounds.Length)];

        if (selected.clip == null)
            return;

        audioSource.pitch = UnityEngine.Random.Range(minPitch, maxPitch);
        audioSource.PlayOneShot(selected.clip, selected.volume);
    }
}