using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private EGameState gameState;

    public static GameManager instance;
    public static EGameState nextState = EGameState.MENU;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetGameState(nextState);

        nextState = EGameState.MENU;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetGameState(EGameState gameState)
    {
        this.gameState = gameState;

        IEnumerable<IGameStateListener> gameStateListeners
            = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<IGameStateListener>();

        foreach (IGameStateListener listener in gameStateListeners)
            listener.GameStateChangedCallback(gameState);
    }

    public void StartGame()
    {
        SetGameState(EGameState.GAME);
    }

    public void NextButtonCallback()
    {
        nextState = EGameState.GAME;
        SceneManager.LoadScene(0);
    }

    public void RetryButtonCallback()
    {
        nextState = EGameState.GAME;
        SceneManager.LoadScene(0);
    }

    public bool IsGame() => gameState == EGameState.GAME;
    
}
