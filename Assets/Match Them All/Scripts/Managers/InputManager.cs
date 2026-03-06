using System;
using UnityEngine;
using UnityEngine.InputSystem;

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
        if (Input.GetMouseButtonDown(0))
            HandleMouseDown();

        if (Mouse.current.leftButton.isPressed)
            HandleDrag();
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
            HandleMouseUp();
    }

    private void HandleMouseDown()
    {
        Physics.Raycast(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()), out RaycastHit hit, 100, powerupLayer);

        if (hit.collider == null)
            return;

        OnPowerupClicked?.Invoke(hit.collider.GetComponent<Powerup>());
    }

    private void HandleDrag()
    {
        Physics.Raycast(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()), out RaycastHit hit, 100);

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
    }
}
