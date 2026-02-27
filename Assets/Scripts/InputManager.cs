using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static Action<Item> OnItemClicked;

    [Header(" Settings ")]
    private Item currentItem;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.leftButton.isPressed)
            HandleDrag();
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
            HandleMouseUp();



    }

    private void HandleDrag()
    {
        Physics.Raycast(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()), out RaycastHit hit, 100);

        if (hit.collider == null)
            return;

        if (hit.collider.transform.parent == null)
        {
            DeselectCurrentItem();
            return;
        }

        if (!hit.collider.transform.parent.TryGetComponent(out Item item))
            return;

        DeselectCurrentItem();

        currentItem = item;
        currentItem.Select();
    }

    private void DeselectCurrentItem()
    {
        if (currentItem != null)
            currentItem.Deselect();

        currentItem = null;
    }

    private void HandleMouseUp()
    {
        if (currentItem == null)
            return;

        currentItem.Deselect();

        OnItemClicked?.Invoke(currentItem);
        currentItem = null;
    }
}
