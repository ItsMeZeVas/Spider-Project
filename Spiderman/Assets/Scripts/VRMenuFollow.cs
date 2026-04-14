using UnityEngine;

public class VRMenuFollow : MonoBehaviour
{
    public Transform playerCamera;
    public float distance = 1.2f;
    public float height = -0.3f;
    public float smoothSpeed = 5f;

    void Update()
    {
        Vector3 forward = playerCamera.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 targetPos =
            playerCamera.position +
            forward * distance +
            Vector3.up * height;

        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            Time.deltaTime * smoothSpeed
        );

        transform.LookAt(playerCamera);
        transform.Rotate(0, 180, 0);
    }
}