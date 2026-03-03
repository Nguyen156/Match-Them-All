using System;
using System.Collections.Generic;
using UnityEngine;

public class GoalManager : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private Transform goalCardParent;
    [SerializeField] private GoalCard goalCardPrefab;

    [Header(" Data ")]
    private ItemLevelData[] goals;
    private List<GoalCard> goalCards = new List<GoalCard>();

    private void Awake()
    {
        LevelManager.OnLevelSpawned += LevelSpawnedCallback;
        ItemSpotsManager.OnItemPickedUp += ItemPickedUpCallback;
    }


    private void OnDestroy()
    {
        LevelManager.OnLevelSpawned -= LevelSpawnedCallback;
        ItemSpotsManager.OnItemPickedUp -= ItemPickedUpCallback;
    }

    private void LevelSpawnedCallback(Level level)
    {
        goals = level.GetGoals();

        GenerateGoalCards();
    }

    private void GenerateGoalCards()
    {
        for (int i = 0; i < goals.Length; i++)
            GenerateGoalCard(goals[i]);
    }

    private void GenerateGoalCard(ItemLevelData goal)
    {
        GoalCard newGoalCard = Instantiate(goalCardPrefab, goalCardParent);

        newGoalCard.Configure(goal.amount);

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

        Debug.Log("Level Complete");
    }
}
