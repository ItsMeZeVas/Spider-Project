using UnityEngine;
using UnityEngine.XR.Hands;           // Necesario

public class WebShooterHands : MonoBehaviour
{
    [Header("Configuración del disparo")]
    public GameObject webProjectilePrefab;     // Prefab de la telaraña/bola
    public Transform shootPoint;               // Punto de salida (punta del dedo índice)
    public float shootForce = 35f;
    public float shootCooldown = 0.35f;

    private float lastShootTime = 0f;

    // Este método lo llamaremos desde el Inspector del XRStaticHandGesture
    public void OnWebShootGesturePerformed()
    {
        if (Time.time - lastShootTime < shootCooldown)
            return;

        if (webProjectilePrefab == null || shootPoint == null)
        {
            Debug.LogWarning("WebShooter: Falta prefab o shootPoint");
            return;
        }

        // Instanciar la telaraña
        GameObject web = Instantiate(webProjectilePrefab, shootPoint.position, shootPoint.rotation);

        // Aplicar velocidad
        Rigidbody rb = web.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = shootPoint.forward * shootForce;
        }

        // Si tienes BallFadeDestroy en la bola, puedes llamarlo aquí
        // BallFadeDestroy fade = web.GetComponent<BallFadeDestroy>();
        // if (fade != null) fade.DestroyOnImpact();

        lastShootTime = Time.time;

        Debug.Log("¡Telaraña disparada con gesto de mano!");
    }

    // Opcional: Método para cuando el gesto termina (si lo necesitas)
    public void OnWebShootGestureEnded()
    {
        // Aquí puedes poner efectos de "soltar" si quieres
        Debug.Log("Gesto de telaraña terminado");
    }
}