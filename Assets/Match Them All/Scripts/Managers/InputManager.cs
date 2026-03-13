using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class InputManager : MonoBehaviour
{
    public static Action<Item> OnItemClicked;
    public static Action<Powerup> OnPowerupClicked;

    [Header(" Settings ")]
    [SerializeField] private LayerMask powerupLayer;
    private Item currentItem;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.IsGame())
            HandleControl();
    }

    private void HandleControl()
    {
        if (Pointer.current.press.wasPressedThisFrame)
            HandleMouseDown();

        if (Pointer.current.press.isPressed)
            HandleDrag();
        else if (Pointer.current.press.wasReleasedThisFrame)
            HandleMouseUp();
    }

    private bool IsPress()
    {
        // Touch (mobile)
        if (Touchscreen.current != null &&
            Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            return true;

        // Mouse (PC)
        if (Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame)
            return true;

        return false;
    }

    private void HandleMouseDown()
    {
        Physics.Raycast(Camera.main.ScreenPointToRay(Pointer.current.position.ReadValue()), out RaycastHit hit, 100, powerupLayer);

        if (hit.collider == null)
            return;

        OnPowerupClicked?.Invoke(hit.collider.GetComponent<Powerup>());
    }

    private void HandleDrag()
    {
        Physics.Raycast(Camera.main.ScreenPointToRay(Pointer.current.position.ReadValue()), out RaycastHit hit, 100);

        if (hit.collider == null)
            return;

        if (hit.collider.transform.parent == null)
            return;

        if (!hit.collider.transform.parent.TryGetComponent(out Item item))
        {
            DeselectCurrentItem();
            return;
        }

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

        AudioManager.instance.PlaySFX(2);
    }
}
