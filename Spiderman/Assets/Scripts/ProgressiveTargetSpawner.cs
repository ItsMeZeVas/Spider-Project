using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressiveTargetSpawner : MonoBehaviour
{
    [Header("Referencias")]
    public GameTimer gameTimer;

    [Header("Prefabs")]
    public GameObject staticTargetPrefab;
    public GameObject movingTargetPrefab;

    [Header("Puntos")]
    public int staticTargetPoints = 5;
    public int movingTargetPoints = 10;

    [Header("Spawn points estáticos")]
    public Transform[] staticSpawnPoints;

    [Header("Waypoints móviles")]
    public Transform[] movingWaypoints;

    [Header("Lógica de partida")]
    public float movingTargetsStartAt = 10f;
    public float checkInterval = 0.4f;

    [Header("Cantidad inicial")]
    public int initialStaticTargets = 3;
    public int initialMovingTargets = 0;

    [Header("Cantidad máxima al final")]
    public int maxStaticTargetsEndgame = 8;
    public int maxMovingTargetsEndgame = 5;

    [Header("Audio de aparición")]
    public AudioSource audioSource;
    public AudioClip spawnClip;
    [Range(0f, 1f)] public float spawnVolume = 0.8f;

    private readonly List<GameObject> activeStaticTargets = new List<GameObject>();
    private readonly List<GameObject> activeMovingTargets = new List<GameObject>();

    void Start()
    {
        if (gameTimer == null)
        {
            Debug.LogWarning("ProgressiveTargetSpawner: falta asignar GameTimer.");
            return;
        }

        gameTimer.OnGameEnded += HandleGameEnded;
        StartCoroutine(SpawnLoop());
    }

    void OnDestroy()
    {
        if (gameTimer != null)
            gameTimer.OnGameEnded -= HandleGameEnded;
    }

    IEnumerator SpawnLoop()
    {
        while (!gameTimer.GameEnded)
        {
            CleanupLists();

            int targetStaticCount = GetDesiredStaticCount();
            int targetMovingCount = GetDesiredMovingCount();

            while (activeStaticTargets.Count < targetStaticCount)
            {
                SpawnStaticTarget();
            }

            while (activeMovingTargets.Count < targetMovingCount)
            {
                SpawnMovingTarget();
            }

            yield return new WaitForSeconds(checkInterval);
        }
    }

    int GetDesiredStaticCount()
    {
        float progress = gameTimer.Progress01;
        return Mathf.RoundToInt(Mathf.Lerp(initialStaticTargets, maxStaticTargetsEndgame, progress));
    }

    int GetDesiredMovingCount()
    {
        if (gameTimer.ElapsedTime < movingTargetsStartAt)
            return 0;

        float adjustedElapsed = gameTimer.ElapsedTime - movingTargetsStartAt;
        float adjustedDuration = Mathf.Max(1f, gameTimer.matchDuration - movingTargetsStartAt);
        float adjustedProgress = Mathf.Clamp01(adjustedElapsed / adjustedDuration);

        return Mathf.RoundToInt(Mathf.Lerp(initialMovingTargets, maxMovingTargetsEndgame, adjustedProgress));
    }

    void CleanupLists()
    {
        activeStaticTargets.RemoveAll(t => t == null);
        activeMovingTargets.RemoveAll(t => t == null);
    }

    void SpawnStaticTarget()
    {
        if (staticTargetPrefab == null || staticSpawnPoints == null || staticSpawnPoints.Length == 0)
            return;

        Transform spawnPoint = GetRandomFreeStaticSpawnPoint();
        if (spawnPoint == null) return;

        GameObject target = Instantiate(staticTargetPrefab, spawnPoint.position, spawnPoint.rotation, transform);
        ConfigureTarget(target, staticTargetPoints, false);
        activeStaticTargets.Add(target);

        PlaySpawnSound();
    }

    void SpawnMovingTarget()
    {
        if (movingTargetPrefab == null || movingWaypoints == null || movingWaypoints.Length < 2)
            return;

        Transform startPoint = movingWaypoints[Random.Range(0, movingWaypoints.Length)];
        GameObject target = Instantiate(movingTargetPrefab, startPoint.position, startPoint.rotation, transform);
        ConfigureTarget(target, movingTargetPoints, true);
        activeMovingTargets.Add(target);

        PlaySpawnSound();
    }

    void ConfigureTarget(GameObject target, int points, bool shouldMove)
    {
        TargetMaterialChanger materialChanger = target.GetComponent<TargetMaterialChanger>();
        if (materialChanger != null)
        {
            materialChanger.pointsValue = points;
        }

        TargetMover mover = target.GetComponent<TargetMover>();
        if (mover != null)
        {
            mover.enabled = shouldMove;

            if (shouldMove)
                mover.waypoints = movingWaypoints;
        }
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

                if (Vector3.Distance(target.transform.position, point.position) < 0.15f)
                {
                    occupied = true;
                    break;
                }
            }

            if (!occupied)
                availablePoints.Add(point);
        }

        if (availablePoints.Count == 0)
            return null;

        return availablePoints[Random.Range(0, availablePoints.Count)];
    }

    void PlaySpawnSound()
    {
        if (audioSource != null && spawnClip != null)
        {
            audioSource.PlayOneShot(spawnClip, spawnVolume);
        }
    }

    void HandleGameEnded()
    {
        StopAllCoroutines();
        DestroyAllTargets();
    }

    void DestroyAllTargets()
    {
        foreach (GameObject target in activeStaticTargets)
        {
            if (target != null)
                Destroy(target);
        }

        foreach (GameObject target in activeMovingTargets)
        {
            if (target != null)
                Destroy(target);
        }

        activeStaticTargets.Clear();
        activeMovingTargets.Clear();
    }
}