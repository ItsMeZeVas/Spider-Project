using UnityEngine;
using System.Collections;

public class TargetMover : MonoBehaviour
{
    [Header("Waypoints (Emptys)")]
    [Tooltip("Arrastra aquí los Empty GameObjects en el orden que quieres que siga")]
    public Transform[] waypoints;

    [Header("Movimiento")]
    [Tooltip("Velocidad de movimiento")]
    public float speed = 3f;

    [Tooltip("Tiempo de espera en cada waypoint (0 = sin pausa)")]
    public float waitTime = 0f;

    [Tooltip("Si es true, vuelve al primer waypoint al terminar (loop)")]
    public bool loop = true;

    private int currentWaypointIndex = 0;
    private bool isMoving = true;

    void Start()
    {
        if (waypoints.Length == 0)
        {
            Debug.LogWarning("TargetMover: No hay waypoints asignados en " + gameObject.name);
            return;
        }

        // Colocar el target en el primer waypoint al inicio
        transform.position = waypoints[0].position;
        StartCoroutine(MoveAlongPath());
    }

    private IEnumerator MoveAlongPath()
    {
        while (isMoving)
        {
            Transform targetWP = waypoints[currentWaypointIndex];

            // Mover hacia el waypoint actual
            while (Vector3.Distance(transform.position, targetWP.position) > 0.05f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetWP.position, speed * Time.deltaTime);
                yield return null;
            }

            // Llegó al waypoint
            transform.position = targetWP.position;

            // Espera opcional
            if (waitTime > 0)
                yield return new WaitForSeconds(waitTime);

            // Avanzar al siguiente waypoint
            currentWaypointIndex++;

            // Si llegó al final
            if (currentWaypointIndex >= waypoints.Length)
            {
                if (loop)
                    currentWaypointIndex = 0;   // Volver al inicio
                else
                    isMoving = false;           // Detenerse
            }
        }
    }

    // Opcional: Pausar / reanudar el movimiento
    public void PauseMovement() => isMoving = false;
    public void ResumeMovement() => isMoving = true;
}