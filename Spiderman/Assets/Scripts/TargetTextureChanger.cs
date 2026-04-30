using UnityEngine;
using System.Collections;

public class TargetMaterialChanger : MonoBehaviour
{
    [Header("Objeto visual")]
    [Tooltip("Arrastra aquí el modelo visual del spiderbot. Si está vacío, usará este objeto.")]
    public Transform visualRoot;

    [Header("Materiales")]
    public Material hitMaterial;

    [Header("Sistema de Puntos")]
    public int pointsValue = 100;

    [Header("Desaparición con Fade")]
    public float fadeDuration = 1.5f;
    public float delayBeforeFade = 0.8f;

    [Header("Opciones")]
    public bool changeOnlyOnce = true;

    private Renderer[] targetRenderers;
    private ScoreManager scoreManager;
    private bool hasBeenHit = false;

    void Awake()
    {
        if (visualRoot == null)
            visualRoot = transform;

        targetRenderers = visualRoot.GetComponentsInChildren<Renderer>();
        scoreManager = FindObjectOfType<ScoreManager>();

        if (targetRenderers == null || targetRenderers.Length == 0)
            Debug.LogWarning($"TargetMaterialChanger: No se encontraron Renderers en {visualRoot.name}");
    }

    public void ChangeMaterial()
    {
        if (hasBeenHit && changeOnlyOnce)
            return;

        if (hitMaterial != null && targetRenderers != null)
        {
            foreach (Renderer r in targetRenderers)
            {
                if (r == null) continue;

                Material[] newMaterials = new Material[r.materials.Length];

                for (int i = 0; i < newMaterials.Length; i++)
                {
                    newMaterials[i] = hitMaterial;
                }

                r.materials = newMaterials;
            }
        }

        if (scoreManager != null)
        {
            scoreManager.AddScore(pointsValue);
        }

        hasBeenHit = true;
        Debug.Log($"{gameObject.name} golpeado → +{pointsValue} puntos");

        StartCoroutine(FadeOutAndDestroy());
    }

    private IEnumerator FadeOutAndDestroy()
    {
        if (delayBeforeFade > 0)
            yield return new WaitForSeconds(delayBeforeFade);

        if (targetRenderers == null || targetRenderers.Length == 0)
        {
            Destroy(gameObject);
            yield break;
        }

        Material[][] allMaterials = new Material[targetRenderers.Length][];
        Color[][] originalColors = new Color[targetRenderers.Length][];

        for (int r = 0; r < targetRenderers.Length; r++)
        {
            if (targetRenderers[r] == null) continue;

            allMaterials[r] = targetRenderers[r].materials;
            originalColors[r] = new Color[allMaterials[r].Length];

            for (int i = 0; i < allMaterials[r].Length; i++)
            {
                if (allMaterials[r][i] == null) continue;

                originalColors[r][i] = allMaterials[r][i].color;
                SetupTransparentMaterial(allMaterials[r][i]);
            }
        }

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);

            for (int r = 0; r < allMaterials.Length; r++)
            {
                if (allMaterials[r] == null) continue;

                for (int i = 0; i < allMaterials[r].Length; i++)
                {
                    if (allMaterials[r][i] == null) continue;

                    Color newColor = originalColors[r][i];
                    newColor.a = alpha;
                    allMaterials[r][i].color = newColor;
                }
            }

            yield return null;
        }

        Destroy(gameObject);
    }

    private void SetupTransparentMaterial(Material mat)
    {
        if (mat == null) return;

        mat.SetFloat("_Surface", 1);
        mat.SetOverrideTag("RenderType", "Transparent");
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;
    }
}