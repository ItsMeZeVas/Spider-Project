using UnityEngine;
using System.Collections;

public class TargetMaterialChanger : MonoBehaviour
{
    [Header("Materiales")]
    public Material normalMaterial;
    public Material hitMaterial;

    [Header("Sistema de Puntos")]
    public int pointsValue = 100;

    [Header("Desaparición con Fade")]
    [Tooltip("Tiempo en segundos que dura el fade out (transparencia)")]
    public float fadeDuration = 1.5f;     // ← Tiempo del fade suave

    [Tooltip("Tiempo adicional de espera después del impacto antes de empezar el fade (0 = inmediato)")]
    public float delayBeforeFade = 0.8f;  // ← Tiempo que se queda con el material "hit" antes de empezar a desaparecer

    [Header("Opciones")]
    public bool changeOnlyOnce = true;

    private Renderer targetRenderer;
    private ScoreManager scoreManager;
    private bool hasBeenHit = false;
    private Material originalMaterial;   // Para guardar referencia si es necesario

    void Awake()
    {
        targetRenderer = GetComponent<Renderer>();
        scoreManager = FindObjectOfType<ScoreManager>();

        if (targetRenderer == null)
            Debug.LogError($"No se encontró Renderer en {gameObject.name}");
    }

    public void ChangeMaterial()
    {
        if (hasBeenHit && changeOnlyOnce)
            return;

        // Cambiar al material de impacto
        if (targetRenderer != null && hitMaterial != null)
        {
            targetRenderer.material = hitMaterial;
        }

        // Sumar puntos
        if (scoreManager != null)
        {
            scoreManager.AddScore(pointsValue);
        }

        hasBeenHit = true;

        Debug.Log($"{gameObject.name} golpeado → +{pointsValue} puntos");

        // Iniciar la rutina de fade + destroy
        StartCoroutine(FadeOutAndDestroy());
    }

    private IEnumerator FadeOutAndDestroy()
    {
        // Esperar el delay antes de empezar el fade
        if (delayBeforeFade > 0)
            yield return new WaitForSeconds(delayBeforeFade);

        if (targetRenderer == null || targetRenderer.material == null)
        {
            Destroy(gameObject);
            yield break;
        }

        Material mat = targetRenderer.material;
        Color originalColor = mat.color;

        // Asegurarnos que el shader permita transparencia
        // (Esto funciona bien en Built-in y URP Lit)
        mat.SetFloat("_Surface", 1);           // 1 = Transparent (URP)
        mat.SetOverrideTag("RenderType", "Transparent");
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);

            Color newColor = originalColor;
            newColor.a = alpha;
            mat.color = newColor;

            yield return null;   // Espera al siguiente frame
        }

        // Asegurar que termine en alpha 0
        Color finalColor = originalColor;
        finalColor.a = 0f;
        mat.color = finalColor;

        // Destruir el objeto
        Destroy(gameObject);
    }

    // Por si quieres resetear (opcional)
    public void ResetMaterial()
    {
        if (targetRenderer != null && normalMaterial != null)
        {
            targetRenderer.material = normalMaterial;
            hasBeenHit = false;
        }
    }
}