using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PowerupManager : MonoBehaviour
{
    [Header(" Vacuum Elements ")]
    [SerializeField] private Vacuum vacuum;
    [SerializeField] private Transform vacuumTargetPos;

    [Header(" Settings ")]
    private bool isBusy;
    private int vacuumItemToCollect;
    private int vacuumCounter;
 
    [Header(" Actions ")]
    public static Action<Item> OnItemPickedUp;

    [Header(" Datas ")]
    [SerializeField] private int initialPUCount;
    private int vacuumPUCount;

    private void Awake()
    {
        LoadData();

        Vacuum.OnStarted += VacuumStartedCallback;
        InputManager.OnPowerupClicked += PowerupClickedCallback;
    }


    private void OnDestroy()
    {
        Vacuum.OnStarted -= VacuumStartedCallback;
        InputManager.OnPowerupClicked -= PowerupClickedCallback;
    }
    private void PowerupClickedCallback(Powerup powerup)
    {
        if(isBusy)
            return;

        switch (powerup.PowerupType)
        {
            case EPowerupType.Vacuum:
                HandleVacuumClicked();
                UpdateVacuumVisuals();

                break;
        }
    }

    private void HandleVacuumClicked()
    {
        if(vacuumPUCount <= 0)
        {
            vacuumPUCount = 3;
            SaveData();
        }
        else
        {
            isBusy = true;

            vacuumPUCount--;
            SaveData();

            vacuum.PlayAnim();
        }

    }

    private void VacuumStartedCallback()
    {
        VacuumPowerup();
    }

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

    private void UpdateVacuumVisuals()
    {
        vacuum.UpdateVisuals(vacuumPUCount);
    }

    private void LoadData()
    {
        vacuumPUCount = PlayerPrefs.GetInt("VacuumCount", initialPUCount);

        UpdateVacuumVisuals();
    }

    private void SaveData()
    {
        PlayerPrefs.SetInt("VacuumCount", vacuumPUCount );
    }
}
