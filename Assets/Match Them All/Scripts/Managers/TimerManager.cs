using System;
using TMPro;
using UnityEngine;

public class TimerManager : MonoBehaviour, IGameStateListener
{
    public static TimerManager instance;

    [Header(" Elements ")]
    [SerializeField] private TextMeshProUGUI timerText;
    private int currentTimer;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        LevelManager.OnLevelSpawned += LevelSpawnedCallback;
    }

    private void OnDestroy()
    {
        LevelManager.OnLevelSpawned -= LevelSpawnedCallback;
    }



    private void LevelSpawnedCallback(Level level)
    {
        currentTimer = level.Duration;
        UpdateTimerText();

        StartTimer();
    }

    private void StartTimer()
    {
        InvokeRepeating(nameof(UpdateTimer), 0, 1);
    }

    private void UpdateTimer()
    {
        currentTimer--;
        UpdateTimerText();

        if (currentTimer <= 0)
            TimerFinished();
    }

    private void UpdateTimerText() => timerText.text = SecondsToString(currentTimer);

    private void TimerFinished()
    {
        GameManager.instance.SetGameState(EGameState.GAMEOVER);
        StopTimer();
    }

    private string SecondsToString(int seconds) 
        => TimeSpan.FromSeconds(seconds).ToString().Substring(3);

    public void GameStateChangedCallback(EGameState gameState)
    {
        if (gameState == EGameState.LEVELCOMPLETE || gameState == EGameState.GAMEOVER)
            StopTimer();
    }

    private void StopTimer()
    {
        CancelInvoke();
    }

    public void FreezeTimer()
    {
        StopTimer();
        Invoke(nameof(StartTimer), 10);
    }
}
