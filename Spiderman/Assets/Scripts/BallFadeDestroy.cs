using UnityEngine;
using System.Collections;

public class BallFadeDestroy : MonoBehaviour
{
    [Header("Tiempo de vida de la bola")]
    [Tooltip("Tiempo total en segundos antes de empezar el fade")]
    public float lifetime = 5f;           // Tiempo que vive antes de empezar a desaparecer

    [Tooltip("Duración del fade out (transparencia)")]
    public float fadeDuration = 1.2f;     // Cuánto dura el fade

    [Header("Opciones")]
    [Tooltip("Si es true, la bola se destruye aunque no haya impactado")]
    public bool destroyEvenIfNotHit = true;

    private Renderer ballRenderer;
    private bool hasStartedFade = false;

    void Awake()
    {
        ballRenderer = GetComponent<Renderer>();

        if (ballRenderer == null)
        {
            Debug.LogError("BallFadeDestroy: No se encontró Renderer en la bola " + gameObject.name);
        }
    }

    void Start()
    {
        // Iniciar el conteo de vida de la bola
        if (destroyEvenIfNotHit)
        {
            StartCoroutine(LifetimeRoutine());
        }
    }

    // Si la bola impacta contra un target, podemos destruirla antes (llamado desde BallCollision)
    public void DestroyOnImpact()
    {
        if (!hasStartedFade)
        {
            StopAllCoroutines();
            StartCoroutine(FadeOutAndDestroy());
        }
    }

    private IEnumerator LifetimeRoutine()
    {
        // Esperar el tiempo de vida normal
        yield return new WaitForSeconds(lifetime);

        // Si aún no ha empezado el fade, iniciarlo
        if (!hasStartedFade)
        {
            StartCoroutine(FadeOutAndDestroy());
        }
    }

    private IEnumerator FadeOutAndDestroy()
    {
        hasStartedFade = true;

        if (ballRenderer == null || ballRenderer.material == null)
        {
            Destroy(gameObject);
            yield break;
        }

        Material mat = ballRenderer.material;
        Color originalColor = mat.color;

        // Configurar el material para que permita transparencia
        SetupTransparentMaterial(mat);

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);

            Color newColor = originalColor;
            newColor.a = alpha;
            mat.color = newColor;

            yield return null;
        }

        // Asegurar que termine completamente transparente
        Color finalColor = originalColor;
        finalColor.a = 0f;
        mat.color = finalColor;

        Destroy(gameObject);
    }

    private void SetupTransparentMaterial(Material mat)
    {
        // Configuración para que funcione el fade en Built-in y URP
        mat.SetFloat("_Surface", 1);                    // Transparent mode (URP)
        mat.SetOverrideTag("RenderType", "Transparent");
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;
    }

    // Opcional: Si quieres cancelar el fade (por si la bola pega muy rápido)
    public void CancelFade()
    {
        StopAllCoroutines();
    }
}