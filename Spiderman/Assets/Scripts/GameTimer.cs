using UnityEngine;
using TMPro;
using System;

public class GameTimer : MonoBehaviour
{
    [Header("Duración de la partida")]
    public float matchDuration = 90f;

    [Header("UI opcional")]
    public TextMeshProUGUI timerText;
    public GameObject endMessageObject;

    private float timeRemaining;
    private bool gameEnded = false;

    public float TimeRemaining => timeRemaining;
    public bool GameEnded => gameEnded;
    public float ElapsedTime => matchDuration - timeRemaining;
    public float Progress01 => Mathf.Clamp01(ElapsedTime / matchDuration);

    public event Action OnGameEnded;

    void Start()
    {
        timeRemaining = matchDuration;
        UpdateTimerUI();

        if (endMessageObject != null)
            endMessageObject.SetActive(false);
    }

    void Update()
    {
        if (gameEnded) return;

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            EndGame();
        }

        UpdateTimerUI();
    }

    void EndGame()
    {
        if (gameEnded) return;

        gameEnded = true;

        if (endMessageObject != null)
            endMessageObject.SetActive(true);

        OnGameEnded?.Invoke();
    }

    void UpdateTimerUI()
    {
        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }
}