using UnityEngine;

public class BallCollision : MonoBehaviour
{
    [Header("Configuración")]
    public string targetTag = "Target";

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(targetTag))
        {
            TargetMaterialChanger target = collision.gameObject.GetComponent<TargetMaterialChanger>();

            if (target != null)
            {
                target.ChangeMaterial();
            }

            // Destruir la bola después del impacto
            Destroy(gameObject, 0.05f);
        }
    }
}