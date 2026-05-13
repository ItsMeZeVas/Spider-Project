using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public FadeManager fadeManager;

    public void IrAPatioDeJuegos()
    {
        StartCoroutine(CargarEscena("Tutorial Gestos"));
    }
    public void IrAlPueblo()
    {
        StartCoroutine(CargarEscena("Interfaz Gestos"));
    }
    public void IrATutorial()
    {
        StartCoroutine(CargarEscena("Tutorial"));
    }
    public void IrAInterfaz()
    {
        StartCoroutine(CargarEscena("Interfaz"));
    }
    IEnumerator CargarEscena(string nombreEscena)
    {
        // Fade a negro
        yield return StartCoroutine(fadeManager.FadeOut());

        // Cargar escena
        SceneManager.LoadScene(nombreEscena);
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
}