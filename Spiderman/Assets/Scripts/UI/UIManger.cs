using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class UIManager1 : MonoBehaviour
{
    [Header("Panels")]
    public GameObject exitPanel;
    public GameObject audioPanel;
    public GameObject tutorialPanel;
    public GameObject startScreen;   // PanelInicio
    public GameObject menuPanel;     // Panel con los BTNs del menú (Salir, Puntuación, Tutorial, Opciones)
    public GameObject PuntuacionPanel;

    [Header("References")]
    public FadeManager fadeManager;

    private bool _vrButtonPressed = false;

    // ??????????????????????????????????????????????
    //  INICIO: muestra panelInicio, oculta menú
    // ??????????????????????????????????????????????
    private void Start()
    {
        // Estado inicial
        startScreen.SetActive(true);
        menuPanel.SetActive(false);
        exitPanel.SetActive(false);
        audioPanel.SetActive(false);
        tutorialPanel.SetActive(false);
        PuntuacionPanel.SetActive(false);

        // Fade de entrada sobre el panelInicio
        StartCoroutine(fadeManager.FadeIn());
    }

    // ??????????????????????????????????????????????
    //  Llamar este método desde el botón del panelInicio
    //  (ej: botón "Comenzar" o "Entrar")
    // ??????????????????????????????????????????????
    public void OnStartScreenConfirm()
    {
        StartCoroutine(TransitionToMenu());
    }

    private IEnumerator TransitionToMenu()
    {
        // 1. Fade a negro
        yield return StartCoroutine(fadeManager.FadeOut());

        // 2. Intercambiar paneles
        startScreen.SetActive(false);
        menuPanel.SetActive(true);

        // 3. Fade desde negro al menú
        yield return StartCoroutine(fadeManager.FadeIn());
    }

    // ??????????????????????????????????????????????
    //  VR Input (sin cambios)
    // ??????????????????????????????????????????????
    private bool IsVRConfirmPressed()
    {
        InputDevice rightHand = GetDevice(XRNode.RightHand);
        if (rightHand.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryRight) && primaryRight)
            return true;
        if (rightHand.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerRight) && triggerRight)
            return true;

        InputDevice leftHand = GetDevice(XRNode.LeftHand);
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

    // ??????????????????????????????????????????????
    //  Menú principal
    // ??????????????????????????????????????????????
    public void ShowExitPanel()
    {
        exitPanel.SetActive(true);
        tutorialPanel.SetActive(false);
    }

    public void HideExitPanel()
    {
        exitPanel.SetActive(false);
    }

    public void OpenAudio()
    {
        audioPanel.SetActive(true);
        tutorialPanel.SetActive(false);
    }

    public void CloseAudio()
    {
        audioPanel.SetActive(false);
    }

    public void OpenVideo()
    {
        tutorialPanel.SetActive(true);
        audioPanel.SetActive(false);
    }

    public void CloseVideo()
    {
        tutorialPanel.SetActive(false);
    }

    public void OpenStats()
    {
        PuntuacionPanel.SetActive(true);
        tutorialPanel.SetActive(false);
    }
    public void CloseStats()
    {
        PuntuacionPanel.SetActive(false);
    }
}