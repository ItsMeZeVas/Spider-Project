using UnityEngine;
using System;
using System.Collections;

public class TutorialPracticeTarget : MonoBehaviour
{
    [Header("Detección")]
    [Tooltip("Tag opcional del proyectil. Si lo dejas vacío, acepta cualquier colisión.")]
    public string projectileTag = "WebProjectile";

    [Header("Feedback visual")]
    public Renderer targetRenderer;
    public Color hitColor = Color.green;
    public float feedbackDuration = 0.25f;
    public float scalePunch = 1.15f;

    [Header("Opcional")]
    public AudioSource audioSource;
    public AudioClip hitSound;

    public static event Action OnAnyPracticeTargetHit;

    private Color originalColor;
    private Vector3 originalScale;
    private bool alreadyHit = false;

    void Awake()
    {
        originalScale = transform.localScale;

        if (targetRenderer == null)
            targetRenderer = GetComponentInChildren<Renderer>();

        if (targetRenderer != null && targetRenderer.material.HasProperty("_Color"))
            originalColor = targetRenderer.material.color;
    }

    void OnCollisionEnter(Collision collision)
    {
        TryRegisterHit(collision.gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        TryRegisterHit(other.gameObject);
    }

    void TryRegisterHit(GameObject other)
    {
        if (alreadyHit)
            return;

        if (!string.IsNullOrEmpty(projectileTag))
        {
            if (!other.CompareTag(projectileTag))
                return;
        }

        alreadyHit = true;

        Debug.Log($"Diana de tutorial impactada: {name}");

        OnAnyPracticeTargetHit?.Invoke();

        StartCoroutine(HitFeedbackAndDestroy());

        Destroy(other, 0.05f);
    }

    IEnumerator HitFeedbackAndDestroy()
    {
        if (audioSource != null && hitSound != null)
            audioSource.PlayOneShot(hitSound);

        if (targetRenderer != null && targetRenderer.material.HasProperty("_Color"))
            targetRenderer.material.color = hitColor;

        transform.localScale = originalScale * scalePunch;

        yield return new WaitForSeconds(feedbackDuration);

        Destroy(gameObject);
    }
}