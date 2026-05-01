using UnityEngine;
using TMPro;

public class LevelUIManager : MonoBehaviour
{
    [Header("Referencias principales")]
    public GameTimer gameTimer;
    public ScoreManager scoreManager;

    [Header("Paneles de la pantalla")]
    public GameObject gameplayPanel;
    public GameObject endPanel;

    [Header("Textos de final")]
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI highScoreText;

    [Header("Opcional: objetos de gameplay que se apagan al finalizar")]
    public GameObject[] objectsToDisableOnEnd;

    private const string HighScoreKey = "SpiderGame_HighScore";

    void Start()
    {
        if (gameplayPanel != null)
            gameplayPanel.SetActive(true);

        if (endPanel != null)
            endPanel.SetActive(false);

        if (gameTimer != null)
            gameTimer.OnGameEnded += HandleGameEnded;
        else
            Debug.LogWarning("LevelUIManager: falta asignar GameTimer.");
    }

    void OnDestroy()
    {
        if (gameTimer != null)
            gameTimer.OnGameEnded -= HandleGameEnded;
    }

    void HandleGameEnded()
    {
        int finalScore = 0;

        if (scoreManager != null)
            finalScore = scoreManager.GetCurrentScore();

        int highScore = PlayerPrefs.GetInt(HighScoreKey, 0);

        if (finalScore > highScore)
        {
            highScore = finalScore;
            PlayerPrefs.SetInt(HighScoreKey, highScore);
            PlayerPrefs.Save();
        }

        if (finalScoreText != null)
            finalScoreText.text = $"Puntaje: {finalScore}";

        if (highScoreText != null)
            highScoreText.text = $"High Score: {highScore}";

        if (gameplayPanel != null)
            gameplayPanel.SetActive(false);

        if (endPanel != null)
            endPanel.SetActive(true);

        foreach (GameObject obj in objectsToDisableOnEnd)
        {
            if (obj != null)
                obj.SetActive(false);
        }
    }
}