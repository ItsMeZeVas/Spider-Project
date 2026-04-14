using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class WebProjectile : MonoBehaviour
{
    private Rigidbody rb;
    private bool hasStuck;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (hasStuck) return;
        hasStuck = true;

        // Detener físicas completamente
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        // Pegarse a la superficie
        transform.SetParent(collision.transform);
    }
}