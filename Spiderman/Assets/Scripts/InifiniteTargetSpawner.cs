using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteTargetSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject staticTargetPrefab;
    public GameObject movingTargetPrefab;

    [Header("Cantidad máxima simultánea")]
    public int maxStaticTargets = 3;
    public int maxMovingTargets = 2;

    [Header("Puntos por tipo")]
    public int staticTargetPoints = 5;
    public int movingTargetPoints = 10;

    [Header("Spawn Points para objetivos estáticos")]
    [Tooltip("Puntos donde pueden aparecer los targets estáticos")]
    public Transform[] staticSpawnPoints;

    [Header("Waypoints para objetivos móviles")]
    [Tooltip("Waypoints que usarán los targets móviles para desplazarse")]
    public Transform[] movingWaypoints;

    [Header("Tiempo entre chequeos de reposición")]
    [Tooltip("Cada cuánto revisa si faltan targets para volver a spawnear")]
    public float respawnCheckInterval = 0.5f;

    private readonly List<GameObject> activeStaticTargets = new List<GameObject>();
    private readonly List<GameObject> activeMovingTargets = new List<GameObject>();

    void Start()
    {
        SpawnInitialTargets();
        StartCoroutine(RespawnLoop());
    }

    void SpawnInitialTargets()
    {
        for (int i = 0; i < maxStaticTargets; i++)
        {
            SpawnStaticTarget();
        }

        for (int i = 0; i < maxMovingTargets; i++)
        {
            SpawnMovingTarget();
        }
    }

    IEnumerator RespawnLoop()
    {
        while (true)
        {
            CleanupLists();

            while (activeStaticTargets.Count < maxStaticTargets)
            {
                SpawnStaticTarget();
            }

            while (activeMovingTargets.Count < maxMovingTargets)
            {
                SpawnMovingTarget();
            }

            yield return new WaitForSeconds(respawnCheckInterval);
        }
    }

    void CleanupLists()
    {
        activeStaticTargets.RemoveAll(target => target == null);
        activeMovingTargets.RemoveAll(target => target == null);
    }

    void SpawnStaticTarget()
    {
        if (staticTargetPrefab == null || staticSpawnPoints == null || staticSpawnPoints.Length == 0)
        {
            Debug.LogWarning("InfiniteTargetSpawner: faltan prefab o spawn points estáticos.");
            return;
        }

        Transform spawnPoint = GetRandomFreeStaticSpawnPoint();
        if (spawnPoint == null)
        {
            Debug.LogWarning("InfiniteTargetSpawner: no hay spawn points estáticos libres.");
            return;
        }

        GameObject target = Instantiate(staticTargetPrefab, spawnPoint.position, spawnPoint.rotation, transform);

        ConfigureTargetPoints(target, staticTargetPoints);
        DisableTargetMovementIfExists(target);

        activeStaticTargets.Add(target);
    }

    void SpawnMovingTarget()
    {
        if (movingTargetPrefab == null || movingWaypoints == null || movingWaypoints.Length < 2)
        {
            Debug.LogWarning("InfiniteTargetSpawner: faltan prefab móvil o waypoints.");
            return;
        }

        Transform startPoint = movingWaypoints[Random.Range(0, movingWaypoints.Length)];
        GameObject target = Instantiate(movingTargetPrefab, startPoint.position, startPoint.rotation, transform);

        ConfigureTargetPoints(target, movingTargetPoints);
        ConfigureTargetMover(target);

        activeMovingTargets.Add(target);
    }

    void ConfigureTargetPoints(GameObject target, int points)
    {
        TargetMaterialChanger materialChanger = target.GetComponent<TargetMaterialChanger>();
        if (materialChanger != null)
        {
            materialChanger.pointsValue = points;
        }
    }

    void DisableTargetMovementIfExists(GameObject target)
    {
        TargetMover mover = target.GetComponent<TargetMover>();
        if (mover != null)
        {
            mover.enabled = false;
        }
    }

    void ConfigureTargetMover(GameObject target)
    {
        TargetMover mover = target.GetComponent<TargetMover>();

        if (mover == null)
        {
            Debug.LogWarning($"InfiniteTargetSpawner: el prefab móvil {target.name} no tiene TargetMover.");
            return;
        }

        mover.enabled = true;
        mover.waypoints = movingWaypoints;
    }

    Transform GetRandomFreeStaticSpawnPoint()
    {
        List<Transform> availablePoints = new List<Transform>();

        foreach (Transform point in staticSpawnPoints)
        {
            if (point == null) continue;

            bool occupied = false;

            foreach (GameObject target in activeStaticTargets)
            {
                if (target == null) continue;

                float distance = Vector3.Distance(target.transform.position, point.position);
                if (distance < 0.1f)
                {
                    occupied = true;
                    break;
                }
            }

            if (!occupied)
            {
                availablePoints.Add(point);
            }
        }

        if (availablePoints.Count == 0)
            return null;

        return availablePoints[Random.Range(0, availablePoints.Count)];
    }
}