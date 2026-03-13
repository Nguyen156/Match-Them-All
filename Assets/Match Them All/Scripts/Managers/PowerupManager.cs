using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

public class PowerupManager : MonoBehaviour, IGameStateListener
{
    [Header(" Vacuum Elements ")]
    [SerializeField] private Vacuum vacuum;
    [SerializeField] private Transform vacuumTargetPos;

    [Header(" Spring Elements ")]
    [SerializeField] private Spring spring;

    [Header(" Fan Elements ")]
    [SerializeField] private Fan fan;

    [Header(" Freeze Elements ")]
    [SerializeField] private Freeze freeze;

    [Header(" Fan Settings ")]
    [SerializeField] private float fanMagnitude;

    [Header(" Settings ")]
    private bool isBusy;
    private int vacuumItemToCollect;
    private int vacuumCounter;
 
    [Header(" Actions ")]
    public static Action<Item> OnItemPickedUp;
    public static Action<Item> OnItemBackToGame;

    [Header(" Powerup Data ")]
    [SerializeField] private int initialPUCount;
    private const string VACUUM_COUNT = "VacuumCount";
    private int vacuumPUCount;
   
    private const string SPRING_COUNT = "SpringCount";
    private int springPUCount;

    private const string FAN_COUNT = "FanCount";
    private int fanPUCount;
   
    private const string FREEZE_COUNT = "FreezeCount";
    private int freezePUCount;

    

    private void Awake()
    {
        LoadData();

        //Vacuum.OnStarted += VacuumStartedCallback;
        InputManager.OnPowerupClicked += PowerupClickedCallback;
    }


    private void OnDestroy()
    {
        //Vacuum.OnStarted -= VacuumStartedCallback;
        InputManager.OnPowerupClicked -= PowerupClickedCallback;
    }
    private void PowerupClickedCallback(Powerup powerup)
    {
        if(isBusy)
            return;

        switch (powerup.PowerupType)
        {
            case EPowerupType.Vacuum:
                if (!CanUsePowerup(vacuumPUCount))
                    break;

                HandlePowerupClicked(VACUUM_COUNT, ref vacuumPUCount);
                VacuumPowerup();
                UpdateVisuals();

                break;

            case EPowerupType.Spring:
                if (!CanUsePowerup(springPUCount))
                    break;
                HandlePowerupClicked(SPRING_COUNT, ref springPUCount);
                SpringPowerup();
                UpdateVisuals();

                break;

            case EPowerupType.Fan:
                if (!CanUsePowerup(fanPUCount))
                    break;
                HandlePowerupClicked(FAN_COUNT, ref fanPUCount);
                FanPowerup();
                UpdateVisuals();

                break;

            case EPowerupType.Freeze:
                if (!CanUsePowerup(freezePUCount))
                    break;
                HandlePowerupClicked(FREEZE_COUNT, ref freezePUCount);
                FreezePowerup();
                UpdateVisuals();

                break;
        }
    }

    private bool CanUsePowerup(int powerupCount) => powerupCount > 0;

    private void HandlePowerupClicked(string key, ref int powerupCount)
    {
        if (powerupCount <= 0)
        {
            return;
            //powerupCount = 3;
            //SaveData(key, powerupCount);
        }
        else
        {
            isBusy = true;

            powerupCount--;

            SaveData(key, powerupCount);

            //Play animation
        }

    }

    /*
    private void VacuumStartedCallback()
    {
        VacuumPowerup();
    }
    */

    #region Vacuum

    [Button]
    private void VacuumPowerup()
    {
        Item[] items = LevelManager.instance.Items;
        ItemLevelData[] goals = GoalManager.instance.Goals;

        ItemLevelData? greatestGoal = GetGreatestGoal(goals);

        if (greatestGoal == null)
            return;

        ItemLevelData goal = (ItemLevelData)greatestGoal;

        

        List<Item> itemsToCollect = new List<Item>();

        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
                continue;

            if (items[i].ItemName == goal.itemPrefab.ItemName)
            {
                itemsToCollect.Add(items[i]);

                if (itemsToCollect.Count >= 3)
                    break;
            }
        }

        vacuumItemToCollect = itemsToCollect.Count;

        for(int i = 0; i < itemsToCollect.Count; i++)
        {
            itemsToCollect[i].DisablePhysics();

            Item itemToCollect = itemsToCollect[i];

            List<Vector3> points = new List<Vector3>();

            points.Add(itemsToCollect[i].transform.position);

            points.Add(itemsToCollect[i].transform.position + Vector3.up * 2);

            points.Add(vacuumTargetPos.position + Vector3.up * 2);

            points.Add(vacuumTargetPos.position);

            LeanTween.moveSpline(itemsToCollect[i].gameObject, points.ToArray(), .75f)
                .setOnComplete(() => ItemReachedVacuum(itemToCollect));

            /*
            LeanTween.move(itemsToCollect[i].gameObject, vacuumTargetPos.position, .5f)
                .setEase(LeanTweenType.easeInCubic)
                .setOnComplete(() => ItemReachedVacuum(itemToCollect));
            */

            LeanTween.scale(itemsToCollect[i].gameObject, Vector3.zero, .75f);
        }

        for(int i = itemsToCollect.Count - 1; i >= 0; i--)
        {
            OnItemPickedUp?.Invoke(itemsToCollect[i]);
            //Destroy(itemsToCollect[i].gameObject);
        }
    }

    private void ItemReachedVacuum(Item item)
    {
        vacuumCounter++;

        if(vacuumCounter >= vacuumItemToCollect)
            isBusy = false;

        Destroy(item.gameObject);
    }

    private ItemLevelData? GetGreatestGoal(ItemLevelData[] goals)
    {
        int max = 0;
        int goalIndex = -1;

        for(int i = 0; i < goals.Length; i++)
        {
            if (goals[i].amount >= max)
            {
                max = goals[i].amount;
                goalIndex = i;
            }
        }

        if (goalIndex <= -1)
            return null;

        return goals[goalIndex];
    }

    private void UpdateVisuals()
    {
        vacuum.UpdateVisuals(vacuumPUCount);
        spring.UpdateVisuals(springPUCount);
        fan.UpdateVisuals(fanPUCount);
        freeze.UpdateVisuals(freezePUCount);
    }

    #endregion

    #region Spring

    [Button]
    public void SpringPowerup()
    {
        Spot spot = ItemSpotsManager.instance.GetLastOccupiedSpot();

        if(spot == null)
            return;

        Item itemToRelease = spot.Item;

        spot.Clear();

        itemToRelease.UnassignSpot();
        itemToRelease.EnablePhysics();
        itemToRelease.EnableShadow();

        itemToRelease.transform.parent = LevelManager.instance.ItemParent;
        itemToRelease.transform.localPosition = Vector3.up * 3;
        itemToRelease.transform.localScale = Vector3.one;

        OnItemBackToGame?.Invoke(itemToRelease);

        Invoke(nameof(PowerupFinished), 1f);
    }

    #endregion

    #region Fan

    [Button]
    public void FanPowerup()
    {
        Item[] items = LevelManager.instance.Items;

        foreach (Item item in items)
        {
            if(item != null)
                item.ApplyRandomForce(fanMagnitude);
        }

        Invoke(nameof(PowerupFinished), 1f);
    }

    #endregion

    #region Freeze

    public void FreezePowerup()
    {
        TimerManager.instance.FreezeTimer();

        Invoke(nameof(PowerupFinished), 1f);
    }

    #endregion

    private void PowerupFinished() => isBusy = false;

    private void LoadData()
    {
        vacuumPUCount = PlayerPrefs.GetInt(VACUUM_COUNT, initialPUCount);
        springPUCount = PlayerPrefs.GetInt(SPRING_COUNT, initialPUCount);
        fanPUCount = PlayerPrefs.GetInt(FAN_COUNT, initialPUCount);
        freezePUCount = PlayerPrefs.GetInt(FREEZE_COUNT, initialPUCount);
       
        UpdateVisuals();
    }

    private void SaveData(string key, int powerupCount)
    {
        PlayerPrefs.SetInt(key, powerupCount);
    }

    public void GameStateChangedCallback(EGameState gameState)
    {
        if (gameState != EGameState.LEVELCOMPLETE)
            return;

        int randomPUIndex = Random.Range(1, 4);

        switch (randomPUIndex)
        {
            case 1:
                vacuumPUCount++;
                SaveData(VACUUM_COUNT, vacuumPUCount);
                break;

            case 2:
                springPUCount++;
                SaveData(SPRING_COUNT, springPUCount);
                break;

            case 3:
                fanPUCount++;
                SaveData(FAN_COUNT, fanPUCount);
                break;

            case 4:
                freezePUCount++;
                SaveData(FREEZE_COUNT, freezePUCount);
                break;

            default:
                break;
        }
    }
}
