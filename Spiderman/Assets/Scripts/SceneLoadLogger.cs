using UnityEngine;
using UnityEngine.SceneManagement;
using System.Diagnostics;

public class SceneLoadLogger : MonoBehaviour
{
    private Stopwatch stopwatch = new Stopwatch();

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    void OnSceneUnloaded(Scene scene)
    {
        stopwatch.Restart();
        UnityEngine.Debug.Log($"⏳ Cargando escena desde: {scene.name}");
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) 
    {
        stopwatch.Stop();
        UnityEngine.Debug.Log($"✅ Escena '{scene.name}' cargada en {stopwatch.ElapsedMilliseconds}ms");
    }
}