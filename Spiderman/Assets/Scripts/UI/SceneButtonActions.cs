using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneButtonActions : MonoBehaviour
{
    [Header("Nombres de escenas")]
    public string mainMenuScene = "MainMenuScene";
    public string tutorialScene = "TutorialScene";
    public string levelScene = "LevelScene";

    [Header("Fade")]
    public SceneFadeTransition fadeTransition;

    public void GoToTutorial()
    {
        LoadScene(tutorialScene);
    }

    public void StartGame()
    {
        LoadScene(levelScene);
    }

    public void RestartLevel()
    {
        LoadScene(levelScene);
    }

    public void GoToMainMenu()
    {
        LoadScene(mainMenuScene);
    }

    public void ExitGame()
    {
        Debug.Log("Salir del juego");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void LoadScene(string sceneName)
    {
        if (fadeTransition != null)
        {
            fadeTransition.LoadSceneWithFade(sceneName);
        }
        else
        {
            Debug.LogWarning("No hay FadeTransition asignado. Cargando escena directamente.");
            SceneManager.LoadScene(sceneName);
        }
    }
}