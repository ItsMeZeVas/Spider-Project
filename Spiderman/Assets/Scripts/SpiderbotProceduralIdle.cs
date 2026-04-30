using System.Collections;
using UnityEngine;

public class SpiderbotProceduralIdle : MonoBehaviour
{
    [Header("Objeto visual del spiderbot")]
    [Tooltip("Arrastra aquí el modelo visual, normalmente Spiderbot_Model")]
    public Transform visualRoot;

    [Header("Pausa entre movimientos")]
    public float minWaitTime = 2f;
    public float maxWaitTime = 5f;

    [Header("Rebote")]
    public float bounceDuration = 0.45f;
    public float bounceHeight = 0.08f;

    [Header("Giro de alerta")]
    public float wiggleDuration = 0.5f;
    public float wiggleAngle = 10f;

    [Header("Pulso")]
    public float pulseDuration = 0.45f;
    public float pulseScale = 1.06f;

    [Header("Opciones")]
    public bool avoidRepeatingSameMove = true;

    private Vector3 originalLocalPosition;
    private Quaternion originalLocalRotation;
    private Vector3 originalLocalScale;

    private int lastMoveIndex = -1;
    private bool isAnimating = false;

    void Start()
    {
        if (visualRoot == null)
        {
            Transform found = transform.Find("Spiderbot_Model");
            visualRoot = found != null ? found : transform;
        }

        originalLocalPosition = visualRoot.localPosition;
        originalLocalRotation = visualRoot.localRotation;
        originalLocalScale = visualRoot.localScale;

        StartCoroutine(IdleLoop());
    }

    private IEnumerator IdleLoop()
    {
        while (true)
        {
            float wait = Random.Range(minWaitTime, maxWaitTime);
            yield return new WaitForSeconds(wait);

            if (isAnimating)
                continue;

            int moveIndex = GetRandomMoveIndex();

            if (moveIndex == 0)
                yield return StartCoroutine(BounceMove());
            else if (moveIndex == 1)
                yield return StartCoroutine(WiggleMove());
            else
                yield return StartCoroutine(PulseMove());

            lastMoveIndex = moveIndex;
        }
    }

    private int GetRandomMoveIndex()
    {
        int totalMoves = 3;

        if (!avoidRepeatingSameMove || lastMoveIndex == -1)
            return Random.Range(0, totalMoves);

        int newIndex;

        do
        {
            newIndex = Random.Range(0, totalMoves);
        }
        while (newIndex == lastMoveIndex);

        return newIndex;
    }

    private IEnumerator BounceMove()
    {
        isAnimating = true;

        float elapsed = 0f;

        while (elapsed < bounceDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / bounceDuration);

            float yOffset = Mathf.Sin(t * Mathf.PI) * bounceHeight;
            visualRoot.localPosition = originalLocalPosition + new Vector3(0f, yOffset, 0f);

            yield return null;
        }

        visualRoot.localPosition = originalLocalPosition;
        isAnimating = false;
    }

    private IEnumerator WiggleMove()
    {
        isAnimating = true;

        float elapsed = 0f;
        float direction = Random.value > 0.5f ? 1f : -1f;

        while (elapsed < wiggleDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / wiggleDuration);

            float angle = Mathf.Sin(t * Mathf.PI * 2f) * wiggleAngle * direction;
            visualRoot.localRotation = originalLocalRotation * Quaternion.Euler(0f, angle, 0f);

            yield return null;
        }

        visualRoot.localRotation = originalLocalRotation;
        isAnimating = false;
    }

    private IEnumerator PulseMove()
    {
        isAnimating = true;

        float elapsed = 0f;
        Vector3 targetScale = originalLocalScale * pulseScale;

        while (elapsed < pulseDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / pulseDuration);

            float pulse = Mathf.Sin(t * Mathf.PI);
            visualRoot.localScale = Vector3.Lerp(originalLocalScale, targetScale, pulse);

            yield return null;
        }

        visualRoot.localScale = originalLocalScale;
        isAnimating = false;
    }
}