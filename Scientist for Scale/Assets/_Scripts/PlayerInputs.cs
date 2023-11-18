using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Input Reader", menuName = "Game/Input Reader")]
public class PlayerInputs : ScriptableObject, ScientistforScale.IPlayerActions
{
    public event UnityAction JumpEvent = delegate { };
    public event UnityAction JumpCancelled = delegate { };
    public event UnityAction<Vector2> MoveEvent = delegate { };
    public event UnityAction MoveCancelled = delegate { };
    public event UnityAction GrowEvent = delegate { };
    public event UnityAction ShrinkEvent = delegate { };
    public event UnityAction GrabEvent = delegate { };
    public event UnityAction ReleaseEvent = delegate { };

    public ScientistforScale gameInput { get; private set; }

    private void OnEnable()
    {
        if (gameInput == null)
        {
            gameInput = new ScientistforScale();
            gameInput.Player.SetCallbacks(this);
        }
        EnableGameplayInput();
    }

    private void OnDisable()
    {
        DisableAllInput();
    }

    private void DisableAllInput()
    {
        gameInput.Player.Disable();
    }

    private void EnableGameplayInput()
    {
        gameInput.Player.Enable();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            MoveEvent?.Invoke(context.ReadValue<Vector2>());
        }
        if (context.phase == InputActionPhase.Canceled)
        {
            MoveCancelled?.Invoke();
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            JumpEvent?.Invoke();
        }
        if (context.phase == InputActionPhase.Canceled)
        {
            JumpCancelled?.Invoke();
        }
    }

    public void OnShrink(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
            ShrinkEvent?.Invoke();
    }

    public void OnGrow(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
            GrowEvent?.Invoke();
    }

    public void OnGrab(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            GrabEvent?.Invoke();
        }
        if (context.phase == InputActionPhase.Canceled)
        {
            ReleaseEvent?.Invoke();
        }
    }
}
