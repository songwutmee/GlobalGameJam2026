using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ButtonController : MonoBehaviour
{
    private PlayerInputActions inputActions;
    private InputAction press;
    public static event Action<int> OnPlayerHit;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        // Debug.Log("[SYSTEM] ButtonController Awake - Script is running.");
    }
    private void OnEnable()
    {
        inputActions.Control.Enable();
        inputActions.Control.LeftD.performed += OnLeftD;
        inputActions.Control.LeftF.performed += OnLeftF;
        inputActions.Control.RightJ.performed += OnRightJ;
        inputActions.Control.RightK.performed += OnRightK;

        // Debug.Log("[SYSTEM] Input Actions Enabled and Subscribed.");
    }

    private void OnDisable()
    {
        inputActions.Control.LeftD.performed -= OnLeftD;
        inputActions.Control.LeftF.performed -= OnLeftF;
        inputActions.Control.RightJ.performed -= OnRightJ;
        inputActions.Control.RightK.performed -= OnRightK;
        inputActions.Control.Disable();
    }
    void Emit(int lane)
    {
        // LOG 1: Check if input is reaching the code
        // Debug.Log($"[INPUT] Key pressed for Lane: {lane} at SongTime: {Conductor.Instance.songPositionSeconds}");
        OnPlayerHit?.Invoke(lane);
    }

    private void OnLeftD(InputAction.CallbackContext ctx) => Emit(0);
    private void OnLeftF(InputAction.CallbackContext ctx) => Emit(1);
    private void OnRightJ(InputAction.CallbackContext ctx) => Emit(2);
    private void OnRightK(InputAction.CallbackContext ctx) => Emit(3);
}

