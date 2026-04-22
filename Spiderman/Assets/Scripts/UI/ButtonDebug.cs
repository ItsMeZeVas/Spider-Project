using UnityEngine;

public class ButtonDebug : MonoBehaviour
{
    public string buttonName = "Botón";

    public void OnButtonPressed()
    {
        Debug.Log($"✅ {buttonName} presionado!");
    }
}