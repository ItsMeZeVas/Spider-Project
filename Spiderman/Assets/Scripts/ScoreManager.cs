using UnityEngine;
using TMPro;        // Si usas TextMeshPro (recomendado)
using UnityEngine.UI; // Si usas UI clásica de Unity

public class ScoreManager : MonoBehaviour
{
    [Header("UI del Score")]
    public TextMeshProUGUI scoreText;     // ← Arrastra aquí tu texto TMP
    // public Text scoreText;             // Descomenta si usas UI Legacy

    private int currentScore = 0;

    public void AddScore(int points)
    {
        currentScore += points;
        UpdateScoreUI();

        Debug.Log($"Score actual: {currentScore}");
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            
            scoreText.text = $"Score: {currentScore}";
        }
    }

    // Para resetear el score (útil al reiniciar nivel)
    public void ResetScore()
    {
        currentScore = 0;
        UpdateScoreUI();
    }

    public int GetCurrentScore()
    {
        return currentScore;
    }
}