using System;
using UnityEngine;

public class ItemSpotsManager : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private Transform itemSpot;

    [Header(" Settings ")]
    [SerializeField] private Vector3 itemLocalPositonOnSpot;
    [SerializeField] private Vector3 itemLocalScaleOnSpot;

    private void Awake()
    {
        InputManager.OnItemClicked += ItemClickedCallback;
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
        item.transform.SetParent(itemSpot);

        item.transform.localPosition = itemLocalPositonOnSpot;
        item.transform.localScale = itemLocalScaleOnSpot;
        item.transform.localRotation = Quaternion.identity;

        item.DisableShadows();

        item.DisablePhysics();
    }
}
