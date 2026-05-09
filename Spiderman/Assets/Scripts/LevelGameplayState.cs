using UnityEngine;

public class LevelGameplayState : MonoBehaviour
{
    [Header("Referencias")]
    public GameTimer gameTimer;
    public WebShooter[] webShooters;

    void Start()
    {
        SetGameplayShooting(true);

        if (gameTimer != null)
        {
            gameTimer.OnGameEnded += HandleGameEnded;
        }
    }

    void OnDestroy()
    {
        if (gameTimer != null)
        {
            gameTimer.OnGameEnded -= HandleGameEnded;
        }
    }

    void HandleGameEnded()
    {
        SetGameplayShooting(false);
    }

    void SetGameplayShooting(bool enabled)
    {
        if (webShooters == null) return;

        foreach (WebShooter shooter in webShooters)
        {
            if (shooter == null) continue;
            shooter.canShootGameplay = enabled;
        }
    }
}