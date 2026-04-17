using System.Collections;
using UnityEngine;

public class TargetSpawnGlow : MonoBehaviour
{
    [Header("Glow al aparecer")]
    public float glowDuration = 0.6f;
    public Color glowColor = Color.cyan;
    public float glowIntensity = 3f;

    private Renderer targetRenderer;
    private Material[] runtimeMaterials;
    private Color[] originalEmissionColors;

    void Awake()
    {
        targetRenderer = GetComponent<Renderer>();

        if (targetRenderer != null)
        {
            runtimeMaterials = targetRenderer.materials;
            originalEmissionColors = new Color[runtimeMaterials.Length];

            for (int i = 0; i < runtimeMaterials.Length; i++)
            {
                if (runtimeMaterials[i] != null)
                {
                    runtimeMaterials[i].EnableKeyword("_EMISSION");

                    if (runtimeMaterials[i].HasProperty("_EmissionColor"))
                        originalEmissionColors[i] = runtimeMaterials[i].GetColor("_EmissionColor");
                }
            }
        }
    }

    void OnEnable()
    {
        StartCoroutine(GlowRoutine());
    }

    IEnumerator GlowRoutine()
    {
        if (runtimeMaterials == null || runtimeMaterials.Length == 0)
            yield break;

        float elapsed = 0f;

        while (elapsed < glowDuration)
        {
            elapsed += Time.deltaTime;
            float t = 1f - (elapsed / glowDuration);

            for (int i = 0; i < runtimeMaterials.Length; i++)
            {
                if (runtimeMaterials[i] != null && runtimeMaterials[i].HasProperty("_EmissionColor"))
                {
                    Color emission = glowColor * glowIntensity * t;
                    runtimeMaterials[i].SetColor("_EmissionColor", emission);
                }
            }

            yield return null;
        }

        for (int i = 0; i < runtimeMaterials.Length; i++)
        {
            if (runtimeMaterials[i] != null && runtimeMaterials[i].HasProperty("_EmissionColor"))
            {
                runtimeMaterials[i].SetColor("_EmissionColor", originalEmissionColors[i]);
            }
        }
    }
}
