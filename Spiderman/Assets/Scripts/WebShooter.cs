using UnityEngine;
using UnityEngine.InputSystem;

public class WebShooter : MonoBehaviour
{
    [Header("Configuración")]
    public GameObject webProjectilePrefab;
    public Transform  shootPoint;
    public float      shootForce = 20f;

    [Header("Munición")]
    public int maxAmmo = 7;

    [Header("Input")]
    [Tooltip("XRI Right/Activate/Value")]
    public InputActionReference triggerAction;
    [Tooltip("XRI Right/Select/Value  (Grip)")]
    public InputActionReference gripAction;

    private int  currentAmmo;
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
                Shoot();
            else
                Debug.Log("Sin telarañas — usa el Grip para recargar");
        }

        triggerWasPressed = triggerPressed;
    }

    void HandleGrip()
    {
        if (gripAction == null) return;

        bool gripPressed = gripAction.action.ReadValue<float>() > 0.5f;

        if (gripPressed && !gripWasPressed && !isReloading)
            StartCoroutine(Reload());

        gripWasPressed = gripPressed;
    }

    void Shoot()
    {
        if (webProjectilePrefab == null || shootPoint == null) return;

        currentAmmo--;
        Debug.Log($"Telarañas restantes: {currentAmmo}/{maxAmmo}");

        GameObject projectile = Instantiate(webProjectilePrefab, shootPoint.position, shootPoint.rotation);

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
            rb.AddForce(shootPoint.forward * shootForce, ForceMode.VelocityChange);
    }

    System.Collections.IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Recargando...");

        yield return new WaitForSeconds(1.5f);

        currentAmmo = maxAmmo;
        isReloading = false;
        Debug.Log($"Recargado! {currentAmmo}/{maxAmmo}");
    }
}