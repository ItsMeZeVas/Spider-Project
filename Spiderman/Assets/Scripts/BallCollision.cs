using UnityEngine;

public class BallCollision : MonoBehaviour
{
    [Header("Configuración")]
    public string targetTag = "Target";

    private BallFadeDestroy ballFade;

    void Awake()
    {
        ballFade = GetComponent<BallFadeDestroy>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(targetTag))
        {
            TargetMaterialChanger target = collision.gameObject.GetComponent<TargetMaterialChanger>();

            if (target != null)
            {
                target.ChangeMaterial();
            }

            // Destruir la bola con fade suave en vez de inmediato
            if (ballFade != null)
            {
                ballFade.DestroyOnImpact();
            }
            else
            {
                Destroy(gameObject, 0.1f);
            }
        }
    }
}