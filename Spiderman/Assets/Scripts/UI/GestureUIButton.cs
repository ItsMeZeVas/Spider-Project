using UnityEngine;

public class GestureUIButton : MonoBehaviour
{
    [Header("Botón")]
    public ButtonDebug buttonDebug;

    [Header("Interacción")]
    public float cooldown = 0.5f;

    private float lastClickTime = -999f;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("IndexFinger"))
            return;

        if (Time.time < lastClickTime + cooldown)
            return;

        lastClickTime = Time.time;

        if (buttonDebug != null)
        {
            buttonDebug.OnButtonPressed();
        }
        else
        {
            Debug.Log("clickeado");
        }
    }
}