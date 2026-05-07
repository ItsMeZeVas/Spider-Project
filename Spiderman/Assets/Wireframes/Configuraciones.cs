using UnityEngine;
using UnityEngine.UI;

public class Configuraciones : MonoBehaviour
{
    public Toggle baja;
    public Toggle media;
    public Toggle alta;

    public Slider musica;
    public Slider efectos;

    int calidad = 1; // 0 baja, 1 media, 2 alta

    void Start()
    {
        CargarConfiguracion();
    }

    public void SetCalidad(int value)
    {
        calidad = value;
    }

    public UIManager uiManager;

    public void Guardar()
    {
        PlayerPrefs.SetInt("Calidad", calidad);
        PlayerPrefs.SetFloat("Musica", musica.value);
        PlayerPrefs.SetFloat("Efectos", efectos.value);

        PlayerPrefs.Save();

        AplicarConfiguracion();

        // Volver al menú con animación
        uiManager.IrMenu();
    }

    public void Restablecer()
    {
        calidad = 1;
        musica.value = 0.5f;
        efectos.value = 0.5f;

        ActualizarUI();
        AplicarConfiguracion();
    }

    void CargarConfiguracion()
    {
        calidad = PlayerPrefs.GetInt("Calidad", 1);
        musica.value = PlayerPrefs.GetFloat("Musica", 0.5f);
        efectos.value = PlayerPrefs.GetFloat("Efectos", 0.5f);

        ActualizarUI();
        AplicarConfiguracion();
    }

    void ActualizarUI()
    {
        baja.isOn = (calidad == 0);
        media.isOn = (calidad == 1);
        alta.isOn = (calidad == 2);
    }

    void AplicarConfiguracion()
    {
        QualitySettings.SetQualityLevel(calidad);

        AudioListener.volume = musica.value;
        // efectos puedes manejarlo con AudioMixer luego (pro)
    }
}