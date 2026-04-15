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
    public float fadeDuration = 1.5f;

    [Tooltip("Tiempo adicional de espera después del impacto antes de empezar el fade")]
    public float delayBeforeFade = 0.8f;

    [Header("Opciones")]
    public bool changeOnlyOnce = true;

    private Renderer targetRenderer;
    private ScoreManager scoreManager;
    private bool hasBeenHit = false;

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

        // Cambiar a hitMaterial en TODOS los slots de material
        if (targetRenderer != null && hitMaterial != null)
        {
            Material[] newMaterials = new Material[targetRenderer.materials.Length];
            for (int i = 0; i < newMaterials.Length; i++)
            {
                newMaterials[i] = hitMaterial;
            }
            targetRenderer.materials = newMaterials;
        }

        // Sumar puntos
        if (scoreManager != null)
        {
            scoreManager.AddScore(pointsValue);
        }

        hasBeenHit = true;
        Debug.Log($"{gameObject.name} golpeado → +{pointsValue} puntos");

        // Iniciar fade en todos los materiales
        StartCoroutine(FadeOutAndDestroy());
    }

    private IEnumerator FadeOutAndDestroy()
    {
        if (delayBeforeFade > 0)
            yield return new WaitForSeconds(delayBeforeFade);

        if (targetRenderer == null || targetRenderer.materials == null || targetRenderer.materials.Length == 0)
        {
            Destroy(gameObject);
            yield break;
        }

        // Guardamos referencia a todos los materiales actuales
        Material[] materials = targetRenderer.materials;
        Color[] originalColors = new Color[materials.Length];

        // Configurar todos los materiales para modo transparente
        for (int i = 0; i < materials.Length; i++)
        {
            if (materials[i] != null)
            {
                originalColors[i] = materials[i].color;
                SetupTransparentMaterial(materials[i]);
            }
        }

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);

            // Aplicar alpha a TODOS los materiales
            for (int i = 0; i < materials.Length; i++)
            {
                if (materials[i] != null)
                {
                    Color newColor = originalColors[i];
                    newColor.a = alpha;
                    materials[i].color = newColor;
                }
            }

            yield return null;
        }

        // Asegurar que termine en alpha 0
        for (int i = 0; i < materials.Length; i++)
        {
            if (materials[i] != null)
            {
                Color finalColor = originalColors[i];
                finalColor.a = 0f;
                materials[i].color = finalColor;
            }
        }

        Destroy(gameObject);
    }

    // Configuración para modo transparente (funciona en Built-in y URP)
    private void SetupTransparentMaterial(Material mat)
    {
        if (mat == null) return;

        mat.SetFloat("_Surface", 1);           // Transparent (URP)
        mat.SetOverrideTag("RenderType", "Transparent");
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;
    }

    public void ResetMaterial()
    {
        if (targetRenderer != null && normalMaterial != null)
        {
            Material[] newMaterials = new Material[targetRenderer.materials.Length];
            for (int i = 0; i < newMaterials.Length; i++)
            {
                newMaterials[i] = normalMaterial;
            }
            targetRenderer.materials = newMaterials;

            hasBeenHit = false;
        }
    }
}