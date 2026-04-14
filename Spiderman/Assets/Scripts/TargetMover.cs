using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TargetMover : MonoBehaviour
{
    [Header("Waypoints (Emptys)")]
    [Tooltip("Arrastra aquí todos los Empty GameObjects")]
    public Transform[] waypoints;

    [Header("Movimiento Random")]
    public float speed = 4f;
    public float minWaitTime = 0.5f;
    public float maxWaitTime = 2f;
    public bool avoidRepeat = true;

    [Header("Evitar Superposición")]
    [Tooltip("Tiempo máximo que esperará si el waypoint está ocupado")]
    public float maxWaitForFreeSlot = 3f;

    // Sistema estático para saber qué waypoints están ocupados
    private static HashSet<Transform> occupiedWaypoints = new HashSet<Transform>();

    private int currentWaypointIndex = 0;
    private bool isMoving = true;
    private Transform currentTargetWP = null;

    void OnDestroy()
    {
        // Liberar el waypoint cuando el objeto se destruya
        if (currentTargetWP != null)
            occupiedWaypoints.Remove(currentTargetWP);
    }

    void Start()
    {
        if (waypoints == null || waypoints.Length < 2)
        {
            Debug.LogWarning("TargetMover: Necesitas al menos 2 waypoints.");
            return;
        }

        // Empezar en un waypoint libre
        currentWaypointIndex = GetRandomFreeWaypoint();
        if (currentWaypointIndex != -1)
        {
            currentTargetWP = waypoints[currentWaypointIndex];
            occupiedWaypoints.Add(currentTargetWP);
            transform.position = currentTargetWP.position;
        }

        StartCoroutine(MoveRandomly());
    }

    private int GetRandomFreeWaypoint()
    {
        int attempts = 0;
        int index;

        do
        {
            index = Random.Range(0, waypoints.Length);
            attempts++;

            // Si después de muchos intentos no encuentra, permite ocupados
            if (attempts > 15)
                return Random.Range(0, waypoints.Length);

        } while (occupiedWaypoints.Contains(waypoints[index]));

        return index;
    }

    private IEnumerator MoveRandomly()
    {
        while (isMoving)
        {
            // Elegir siguiente waypoint libre
            int nextIndex = GetRandomFreeWaypoint();

            if (nextIndex == -1)
            {
                // Si no hay ninguno libre, esperar un poco
                yield return new WaitForSeconds(0.5f);
                continue;
            }

            // Liberar el waypoint actual antes de movernos
            if (currentTargetWP != null)
                occupiedWaypoints.Remove(currentTargetWP);

            currentWaypointIndex = nextIndex;
            currentTargetWP = waypoints[currentWaypointIndex];
            occupiedWaypoints.Add(currentTargetWP);

            Transform targetWP = currentTargetWP;

            // Mover hacia el waypoint
            while (Vector3.Distance(transform.position, targetWP.position) > 0.05f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetWP.position, speed * Time.deltaTime);
                yield return null;
            }

            transform.position = targetWP.position;

            // Espera random
            float wait = Random.Range(minWaitTime, maxWaitTime);
            yield return new WaitForSeconds(wait);
        }
    }

    public void PauseMovement() => isMoving = false;
    public void ResumeMovement() => isMoving = true;
}