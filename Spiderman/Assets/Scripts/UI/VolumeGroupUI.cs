using UnityEngine;
using TMPro;

public class VolumeGroupUI : MonoBehaviour
{
    public string parameterName; // "MasterVol", etc
    public TextMeshProUGUI valueText;

    private float currentValue = 100f;

    void Start()
    {
        currentValue = PlayerPrefs.GetFloat(parameterName, 100f);
        ApplyVolume();
    }

    public void Increase()
    {
        currentValue = Mathf.Clamp(currentValue + 10f, 0f, 100f);
        ApplyVolume();

    }

    public void Decrease()
    {
        currentValue = Mathf.Clamp(currentValue - 10f, 0f, 100f);
        ApplyVolume();
    }

    void ApplyVolume()
    {
        valueText.text = currentValue + "%";

        // Conversión a decibelios
        float db = Mathf.Log10(Mathf.Max(currentValue / 100f, 0.0001f)) * 20f;

        AudioManager.Instance.SetVolume(parameterName, db);

        PlayerPrefs.SetFloat(parameterName, currentValue);
    }
}