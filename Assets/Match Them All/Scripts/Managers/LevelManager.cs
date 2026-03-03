using System;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header(" Data ")]
    [SerializeField] private Level[] levels;
    private const string LEVEL_KEY = "Level";
    private int levelIndex;

    [Header(" Settings ")]
    private Level currentLevel;

    [Header(" Actions ")]
    public static Action<Level> OnLevelSpawned;

    private void Awake()
    {
        LoadData();
    }

    private void Start()
    {
        SpawnLevel();
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
}
