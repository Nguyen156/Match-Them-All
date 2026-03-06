using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpotsManager : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private Transform itemSpotsParent;
    private Spot[] spots;

    [Header(" Settings ")]
    [SerializeField] private Vector3 itemLocalPositonOnSpot;
    [SerializeField] private Vector3 itemLocalScaleOnSpot;
    private bool isBusy;

    [Header(" Data ")]
    private Dictionary<EItemName, ItemMergeData> itemMergeDataDictionary = new Dictionary<EItemName, ItemMergeData>();

    [Header(" Animation Settings ")]
    [SerializeField] private float animDuration = .15f;
    [SerializeField] private LeanTweenType animEasing;

    [Header(" Actions ")]
    public static Action<List<Item>> OnMergeStarted;
    public static Action<Item> OnItemPickedUp;

    private void Awake()
    {
        InputManager.OnItemClicked += ItemClickedCallback;

        StoreSpots();
    }

    private void OnDestroy()
    {
        InputManager.OnItemClicked -= ItemClickedCallback;
    }

    private void ItemClickedCallback(Item item)
    {
        if (isBusy)
        {
            Debug.Log("Item spot manager is busy !!!");
            return;
        }

        if (!IsFreeSpotAvailabe())
        {
            return;
        }

        isBusy = true;

        OnItemPickedUp?.Invoke(item);

        HandleItemClicked(item);
    }

    private void HandleItemClicked(Item item)
    {
        if (itemMergeDataDictionary.ContainsKey(item.ItemName))
            HandleItemMergeDataFound(item);
        else
            MoveItemToFirstFreeSpot(item);
    }

    private void HandleItemMergeDataFound(Item item)
    {
        Spot idealSpot = GetIdealSpotFor(item);

        itemMergeDataDictionary[item.ItemName].Add(item);

        TryMoveItemToIdealSpot(item, idealSpot);
    }

    private Spot GetIdealSpotFor(Item item)
    {
        List<Item> items = itemMergeDataDictionary[item.ItemName].items;
        List<Spot> spotList = new List<Spot>();

        for (int i = 0; i < items.Count; i++)
            spotList.Add(items[i].Spot);

        if (spotList.Count >= 2)
            spotList.Sort((a, b) => b.transform.GetSiblingIndex().CompareTo(a.transform.GetSiblingIndex()));

        int idealSpotIndex = spotList[0].transform.GetSiblingIndex() + 1;

        return spots[idealSpotIndex];
    }

    private void TryMoveItemToIdealSpot(Item item, Spot idealSpot)
    {
        if (!idealSpot.IsEmpty())
        {
            HandleIdealSpotFull(item, idealSpot);
            return;
        }

        MoveItemToSpot(item, idealSpot, () => HandleItemReachedSpot(item));
    }

    private void MoveItemToSpot(Item item, Spot targetSpot, Action completeCallback)
    {
        targetSpot.Populate(item);

        //item.transform.localPosition = itemLocalPositonOnSpot;
        //item.transform.localScale = itemLocalScaleOnSpot;
        //item.transform.localRotation = Quaternion.identity;

        LeanTween.moveLocal(item.gameObject, itemLocalPositonOnSpot, animDuration)
            .setEase(animEasing);

        LeanTween.scale(item.gameObject, itemLocalScaleOnSpot, animDuration)
            .setEase(animEasing);

        LeanTween.rotateLocal(item.gameObject, Vector3.zero, animDuration)
            .setOnComplete(completeCallback);

        item.DisableShadows();

        item.DisablePhysics();
    }

    private void HandleItemReachedSpot(Item item, bool checkForMerge = true)
    {
        item.Spot.BumpDown();

        if (!checkForMerge)
            return;

        if (itemMergeDataDictionary[item.ItemName].CanMergeItems())
            MergeItems(itemMergeDataDictionary[item.ItemName]);
        else
            CheckForGameover();
    }

    private void MergeItems(ItemMergeData itemMergeData)
    {
        List<Item> itemList = itemMergeData.items;

        //Remove item merge data from the dictionary
        itemMergeDataDictionary.Remove(itemMergeData.itemName);

        for (int i = 0; i < itemList.Count; i++)
            itemList[i].Spot.Clear();

        if(itemMergeDataDictionary.Count <= 0)
            isBusy = false;
        else
            MoveAllItemsToTheLeft(HandleAllItemsMovedToTheLeft);

        OnMergeStarted?.Invoke(itemList);
    }

    private void MoveAllItemsToTheLeft(Action completeCallback)
    {
        bool callbackTriggered = false;

        for (int i = 3; i < spots.Length; i++)
        {
            Spot spot = spots[i];

            if (spot.IsEmpty())
                continue;

            Item item = spot.Item;

            Spot targetSpot = spots[i - 3];

            if (!targetSpot.IsEmpty())
            {
                isBusy = false;
                return;
            }

            spot.Clear();

            completeCallback += () => HandleItemReachedSpot(item, false);
            MoveItemToSpot(item, targetSpot, completeCallback);

            callbackTriggered = true;
        }

        if(!callbackTriggered)
            completeCallback?.Invoke();
    }

    private void HandleAllItemsMovedToTheLeft()
    {
        isBusy = false;
    }

    private void HandleIdealSpotFull(Item item, Spot idealSpot)
    {
        MoveAllItemsToTheRightFrom(idealSpot, item);
    }

    private void MoveAllItemsToTheRightFrom(Spot idealSpot, Item itemToPlace)
    {
        int idealSpotIndex = idealSpot.transform.GetSiblingIndex();

        for (int i = spots.Length - 2; i >= idealSpotIndex; i--)
        {
            if (spots[i].IsEmpty())
                continue;

            Item item = spots[i].Item;

            spots[i].Clear();

            Spot targetSpot = spots[i + 1];

            if (!targetSpot.IsEmpty())
            {
                Debug.LogWarning("This should not happen!!!");
                isBusy = false;
                return;
            }

            MoveItemToSpot(item, targetSpot, () => HandleItemReachedSpot(item, false));
        }

        MoveItemToSpot(itemToPlace, idealSpot, () => HandleItemReachedSpot(itemToPlace));
    }

    private void MoveItemToFirstFreeSpot(Item item)
    {
        Spot targetSpot = GetFreeSpot();

        if (targetSpot == null)
            return;

        CreateItemMergeData(item);

        MoveItemToSpot(item, targetSpot, () => HandleFirstItemReachedSpot(item));
    }

    private void HandleFirstItemReachedSpot(Item item)
    {
        item.Spot.BumpDown();

        CheckForGameover();
    }

    private void CheckForGameover()
    {
        if (GetFreeSpot() == null)
            GameManager.instance.SetGameState(EGameState.GAMEOVER);
        else
            isBusy = false;
    }

    private void CreateItemMergeData(Item item)
    {
        itemMergeDataDictionary.Add(item.ItemName, new ItemMergeData(item));
    }

    private void StoreSpots()
    {
        spots = new Spot[itemSpotsParent.childCount];

        for (int i = 0; i < spots.Length; i++)
            spots[i] = itemSpotsParent.GetChild(i).GetComponent<Spot>();
    }

    private Spot GetFreeSpot()
    {
        for (int i = 0; i < spots.Length; i++)
        {
            if (spots[i].IsEmpty())
                return spots[i];
        }

        return null;
    }

    private bool IsFreeSpotAvailabe()
    {
        for (int i = 0; i < spots.Length; i++)
        {
            if (spots[i].IsEmpty())
                return true;
        }

        return false;
    }
}
