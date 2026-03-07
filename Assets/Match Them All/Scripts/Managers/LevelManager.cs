using System;
using TMPro;
using UnityEngine;

public class LevelManager : MonoBehaviour, IGameStateListener
{
    public static LevelManager instance;

    [Header(" Data ")]
    [SerializeField] private Level[] levels;
    private const string LEVEL_KEY = "Level";
    private int levelIndex;
    public Item[] Items => currentLevel.GetItems();
    public Transform ItemParent => currentLevel.ItemParent;

    [Header(" Settings ")]
    [SerializeField] private TextMeshProUGUI levelText;
    private Level currentLevel;

    [Header(" Actions ")]
    public static Action<Level> OnLevelSpawned;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        LoadData();
    }

    private void Start()
    {
        
    }

    private void SpawnLevel()
    {
        transform.Clear();

        int validatedLevelIndex = levelIndex % levels.Length;

        currentLevel = Instantiate(levels[validatedLevelIndex], transform);

        OnLevelSpawned?.Invoke(currentLevel);
    }

    private void LoadData()
    {
        levelIndex = PlayerPrefs.GetInt(LEVEL_KEY);
    }

    private void SaveData()
    {
        PlayerPrefs.SetInt(LEVEL_KEY, levelIndex);
    }

    private void UpdateLevelText() => levelText.text = "Level " + (levelIndex + 1).ToString();

    public void GameStateChangedCallback(EGameState gameState)
    {
        if(gameState == EGameState.GAME)
        {
            SpawnLevel();
            UpdateLevelText();
        }
        else if(gameState == EGameState.LEVELCOMPLETE)
        {
            levelIndex++;
            SaveData();
        }
    }
}
