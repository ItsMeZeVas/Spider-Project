using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Diagnostics;

public class SceneLoader : MonoBehaviour
{
    private SceneFadeTransition fadeTransition;
    private Stopwatch stopwatch = new Stopwatch();
    private static SceneLoader instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        fadeTransition = FindObjectOfType<SceneFadeTransition>();

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    void OnDestroy()
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

    public void IrAPatioDeJuegos() => fadeTransition.LoadSceneWithFade("Tutorial Gestos");
    public void IrAlPueblo()       => fadeTransition.LoadSceneWithFade("Interfaz Gestos");
    public void IrAPartida()       => fadeTransition.LoadSceneWithFade("Level Gestos");
    public void IrATutorial()      => fadeTransition.LoadSceneWithFade("Tutorial");
    public void IrAInterfaz()      => fadeTransition.LoadSceneWithFade("Interfaz");

    public void ExitGame()
    {
        UnityEngine.Debug.Log("Salir del juego");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}