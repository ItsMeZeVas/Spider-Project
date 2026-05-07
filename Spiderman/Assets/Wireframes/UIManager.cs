using UnityEngine;
using UnityEngine.XR;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public GameObject startScreen;
    public GameObject menuPrincipal;
    public GameObject configuraciones;
    public GameObject tutorial;

    public FadeManager fadeManager;

    private bool _vrButtonPressed = false;

    void Update()
    {
        if (startScreen.activeSelf && IsVRConfirmPressed())
        {
            StartCoroutine(CambiarPantalla(ShowMenu));
        }
    }

    /// <summary>
    /// Detecta botón de confirmación en controladores VR.
    /// Compatible con Meta Quest, HTC Vive y OpenXR genérico.
    /// </summary>
    private bool IsVRConfirmPressed()
    {
        // --- Controlador derecho ---
        InputDevice rightHand = GetDevice(XRNode.RightHand);

        // Botón A (Meta Quest / OpenXR)
        if (rightHand.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryRight) && primaryRight)
            return true;

        // Trigger derecho presionado completamente
        if (rightHand.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerRight) && triggerRight)
            return true;

        // --- Controlador izquierdo ---
        InputDevice leftHand = GetDevice(XRNode.LeftHand);

        // Botón X (Meta Quest / OpenXR)
        if (leftHand.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryLeft) && primaryLeft)
            return true;

        return false;
    }

    private InputDevice GetDevice(XRNode node)
    {
        var devices = new List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(node, devices);
        return devices.Count > 0 ? devices[0] : default;
    }

    // ─── Pantallas ────────────────────────────────────────────────

    public void ShowComienzo()
    {
        startScreen.SetActive(true);
        menuPrincipal.SetActive(false);
        configuraciones.SetActive(false);
        tutorial.SetActive(false);
    }

    public void ShowMenu()
    {
        startScreen.SetActive(false);
        menuPrincipal.SetActive(true);
        configuraciones.SetActive(false);
        tutorial.SetActive(false);
    }

    public void ShowConfiguraciones()
    {
        startScreen.SetActive(false);
        menuPrincipal.SetActive(false);
        configuraciones.SetActive(true);
        tutorial.SetActive(false);
    }

    public void ShowTutorial()
    {
        startScreen.SetActive(false);
        menuPrincipal.SetActive(false);
        configuraciones.SetActive(false);
        tutorial.SetActive(true);
    }

    IEnumerator CambiarPantalla(System.Action accion)
    {
        yield return StartCoroutine(fadeManager.FadeOut());
        accion.Invoke();
        yield return StartCoroutine(fadeManager.FadeIn());
    }

    // ─── Métodos para botones ─────────────────────────────────────

    public void IrMenu() => StartCoroutine(CambiarPantalla(ShowMenu));
    public void IrConfiguraciones() => StartCoroutine(CambiarPantalla(ShowConfiguraciones));
    public void IrTutorial() => StartCoroutine(CambiarPantalla(ShowTutorial));
    public void IrComienzo() => StartCoroutine(CambiarPantalla(ShowComienzo));
}