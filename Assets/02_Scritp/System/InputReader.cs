using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerInputAction;

[CreateAssetMenu(fileName = "New Input Reader", menuName = "SO/Input")]

public class InputReader : ScriptableObject, IPlayerActions
{
    public event Action<Vector2> MovementEvent;
    public event Action<bool> LeftClickEvent;

    private PlayerInputAction inputActions;

    private void OnEnable()
    {
        if (inputActions == null)
        {
            inputActions = new PlayerInputAction();
            inputActions.Player.SetCallbacks(this);
        }
        inputActions.Player.Enable();
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            LeftClickEvent?.Invoke(true);
        }
        else if (context.canceled)
        {
            LeftClickEvent?.Invoke(false);
        }
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        Vector2 moveVecter = context.ReadValue<Vector2>();
        MovementEvent?.Invoke(moveVecter);
    }
}
