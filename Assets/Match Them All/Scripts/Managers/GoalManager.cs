using System;
using System.Collections.Generic;
using UnityEngine;

public class GoalManager : MonoBehaviour
{
    public static GoalManager instance;

    [Header(" Elements ")]
    [SerializeField] private Transform goalCardParent;
    [SerializeField] private GoalCard goalCardPrefab;

    [Header(" Data ")]
    private ItemLevelData[] goals;
    private List<GoalCard> goalCards = new List<GoalCard>();

    public ItemLevelData[] Goals => this.goals;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        LevelManager.OnLevelSpawned += LevelSpawnedCallback;
        ItemSpotsManager.OnItemPickedUp += ItemPickedUpCallback;

        PowerupManager.OnItemPickedUp += ItemPickedUpCallback;
        PowerupManager.OnItemBackToGame += ItemBackToGameCallback;
    }


    private void OnDestroy()
    {
        LevelManager.OnLevelSpawned -= LevelSpawnedCallback;
        ItemSpotsManager.OnItemPickedUp -= ItemPickedUpCallback;
        PowerupManager.OnItemPickedUp -= ItemPickedUpCallback;
        PowerupManager.OnItemBackToGame -= ItemBackToGameCallback;
    }

    private void ItemBackToGameCallback(Item releasedItem)
    {
        for(int i = 0; i < goals.Length; i++)
        {
            if (goals[i].itemPrefab.ItemName != releasedItem.ItemName)
                continue;

            goals[i].amount++;
            goalCards[i].UpdateAmount(goals[i].amount);
        }
    }

    private void LevelSpawnedCallback(Level level)
    {
        
        goals = level.GetGoals();

        GenerateGoalCards();
    }

    private void GenerateGoalCards()
    {
        transform.Clear();

        for (int i = 0; i < goals.Length; i++)
            GenerateGoalCard(goals[i]);
    }

    private void GenerateGoalCard(ItemLevelData goal)
    {
        GoalCard newGoalCard = Instantiate(goalCardPrefab, goalCardParent);

        newGoalCard.Configure(goal.amount, goal.itemPrefab.Icon);

        goalCards.Add(newGoalCard);
    }

    private void ItemPickedUpCallback(Item item)
    {
        for(int i = 0; i < goals.Length; i++)
        {
            if (!goals[i].itemPrefab.ItemName.Equals(item.ItemName))
                continue;

            goals[i].amount--;

            if (goals[i].amount <= 0)
                CompleteGoal(i);
            else
                goalCards[i].UpdateAmount(goals[i].amount);

            break;
        }
    }

    private void CompleteGoal(int goalIndex)
    {
        Debug.Log("Goal complete: " + goals[goalIndex].itemPrefab.ItemName);

        goalCards[goalIndex].Complete();

        CheckForLevelComplete();
    }

    private void CheckForLevelComplete()
    {
        for(int i = 0;i < goals.Length; i++)
        {
            if(goals[i].amount > 0)
                return;
        }

        Invoke(nameof(SetLevelComplete), 3f);
    }

    private void SetLevelComplete() 
        => GameManager.instance.SetGameState(EGameState.LEVELCOMPLETE);
}
