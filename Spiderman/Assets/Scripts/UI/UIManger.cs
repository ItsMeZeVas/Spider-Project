using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject exitPanel;
    public GameObject audioPanel;
    public GameObject tutorialPanel;

    public void ShowExitPanel()
    {
        exitPanel.SetActive(true);
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
    }

    public void CloseVideo()
    {
        tutorialPanel.SetActive(false);
    }
}