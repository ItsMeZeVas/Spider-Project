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

    IEnumerator CargarEscena(string nombreEscena)
    {
        // Fade a negro
        yield return StartCoroutine(fadeManager.FadeOut());

        // Cargar escena
        SceneManager.LoadScene(nombreEscena);
    }
}