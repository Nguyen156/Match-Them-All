using System;
using UnityEngine;

public class ItemSpotsManager : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private Transform itemSpotsParent;
    private Spot[] spots;

    [Header(" Settings ")]
    [SerializeField] private Vector3 itemLocalPositonOnSpot;
    [SerializeField] private Vector3 itemLocalScaleOnSpot;

    private void Awake()
    {
        InputManager.OnItemClicked += ItemClickedCallback;

        StoreSpots();
    }

    private void OnDestroy()
    {
        InputManager.OnItemClicked -= ItemClickedCallback;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void ItemClickedCallback(Item item)
    {
        if (!IsFreeSpotAvailabe())
        {
            return;
        }

        HandleItemClicked(item);

       
    }

    private void HandleItemClicked(Item item)
    {
        MoveItemToFirstFreeSpot(item);
    }

    private void MoveItemToFirstFreeSpot(Item item)
    {
        Spot targetSpot = GetFreeSpot();

        targetSpot.Populate(item);

        item.transform.localPosition = itemLocalPositonOnSpot;
        item.transform.localScale = itemLocalScaleOnSpot;
        item.transform.localRotation = Quaternion.identity;

        item.DisableShadows();

        item.DisablePhysics();
    }

    private void StoreSpots()
    {
        spots = new Spot[itemSpotsParent.childCount];

        for (int i = 0; i < spots.Length; i++)
            spots[i] = itemSpotsParent.GetChild(i).GetComponent<Spot>();
    }

    private Spot GetFreeSpot()
    {
        for(int i = 0;i < spots.Length; i++)
        {
            if (spots[i].IsEmpty())
                return spots[i];
        }

        return null;
    }

    private bool IsFreeSpotAvailabe()
    {
        for (int i = 0;i < spots.Length; i++)
        {
            if (spots[i].IsEmpty())
                return true;
        }

        return false;
    }
}
