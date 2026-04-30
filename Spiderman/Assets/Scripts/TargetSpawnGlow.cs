using System.Collections;
using UnityEngine;

public class TargetSpawnGlow : MonoBehaviour
{
    [Header("Objeto visual")]
    [Tooltip("Arrastra aquí el modelo visual del spiderbot. Si está vacío, usará este objeto.")]
    public Transform visualRoot;

    [Header("Glow al aparecer")]
    public float glowDuration = 0.6f;
    public Color glowColor = Color.cyan;
    public float glowIntensity = 3f;

    private Renderer[] targetRenderers;
    private Material[][] runtimeMaterials;
    private Color[][] originalEmissionColors;

    void Awake()
    {
        if (visualRoot == null)
            visualRoot = transform;

        targetRenderers = visualRoot.GetComponentsInChildren<Renderer>();

        runtimeMaterials = new Material[targetRenderers.Length][];
        originalEmissionColors = new Color[targetRenderers.Length][];

        for (int r = 0; r < targetRenderers.Length; r++)
        {
            if (targetRenderers[r] == null) continue;

            runtimeMaterials[r] = targetRenderers[r].materials;
            originalEmissionColors[r] = new Color[runtimeMaterials[r].Length];

            for (int i = 0; i < runtimeMaterials[r].Length; i++)
            {
                Material mat = runtimeMaterials[r][i];

                if (mat == null) continue;

                mat.EnableKeyword("_EMISSION");

                if (mat.HasProperty("_EmissionColor"))
                    originalEmissionColors[r][i] = mat.GetColor("_EmissionColor");
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

            for (int r = 0; r < runtimeMaterials.Length; r++)
            {
                if (runtimeMaterials[r] == null) continue;

                for (int i = 0; i < runtimeMaterials[r].Length; i++)
                {
                    Material mat = runtimeMaterials[r][i];

                    if (mat != null && mat.HasProperty("_EmissionColor"))
                    {
                        Color emission = glowColor * glowIntensity * t;
                        mat.SetColor("_EmissionColor", emission);
                    }
                }
            }

            yield return null;
        }

        for (int r = 0; r < runtimeMaterials.Length; r++)
        {
            if (runtimeMaterials[r] == null) continue;

            for (int i = 0; i < runtimeMaterials[r].Length; i++)
            {
                Material mat = runtimeMaterials[r][i];

                if (mat != null && mat.HasProperty("_EmissionColor"))
                {
                    mat.SetColor("_EmissionColor", originalEmissionColors[r][i]);
                }
            }
        }
    }
}