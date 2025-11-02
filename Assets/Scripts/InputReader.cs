 using System;
 using UnityEngine;
 using UnityEngine.InputSystem;
 [CreateAssetMenu(fileName = "InputReader", menuName = "Input/Input Reader")]
 public class InputReader : ScriptableObject, Controls.IPlayerActions
 {
    private Controls _controls;
    public event Action<Vector2> MoveEvent;
    public event Action<bool> PrimaryFireEvent;
    private void OnEnable()
    {
        if (_controls == null)
        {
            _controls = new Controls();
            _controls.Player.SetCallbacks(this);
        }
        _controls.Player.Enable();
    }
    private void OnDisable()
    {
        if (_controls != null)
            _controls.Player.Disable();
    }
    public void OnMove(InputAction.CallbackContext context)
        => MoveEvent?.Invoke(context.ReadValue<Vector2>());
    public void OnPrimaryFire(InputAction.CallbackContext context)
    {
        if (context.performed)  PrimaryFireEvent?.Invoke(true);
        if (context.canceled)   PrimaryFireEvent?.Invoke(false);
    }
 }
