using UnityEngine;
using UnityEngine.SceneManagement;

public class TargetMaterialChanger : MonoBehaviour
{
    [Header("Materiales")]
    public Material normalMaterial;
    public Material hitMaterial;

    [Header("Sistema de Puntos")]
    public int pointsValue = 100;        // ← Aquí decides cuántos puntos da este objetivo

    [Header("Opciones")]
    public bool changeOnlyOnce = true;

    private Renderer targetRenderer;
    private bool hasBeenHit = false;

    // Referencia al Score Manager (se buscará automáticamente)
    private ScoreManager scoreManager;

    void Awake()
    {
        targetRenderer = GetComponent<Renderer>();
        scoreManager = FindObjectOfType<ScoreManager>();

        if (targetRenderer == null)
            Debug.LogError($"No se encontró Renderer en {gameObject.name}");

        if (scoreManager == null)
            Debug.LogWarning($"ScoreManager no encontrado en la escena. Los puntos no se sumarán.");
    }

    public void ChangeMaterial()
    {
        if (hasBeenHit && changeOnlyOnce)
            return;

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
    }

    public void ResetMaterial()
    {
        if (targetRenderer != null && normalMaterial != null)
        {
            targetRenderer.material = normalMaterial;
            hasBeenHit = false;
        }
    }
}