using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum NoteType
{
    LeftD,
    LeftF,
    RightJ,
    RightK
}


public class ButtonController : MonoBehaviour
{
    private PlayerInputActions inputActions;
    private InputAction press;
    public static event Action<int> OnPlayerHit;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
    }
    private void OnEnable()
    {
        inputActions.Control.Enable();
        inputActions.Control.LeftD.performed += _ => Emit(0);
        inputActions.Control.LeftF.performed += _ => Emit(1);
        inputActions.Control.RightJ.performed += _ => Emit(2);
        inputActions.Control.RightK.performed += _ => Emit(3);
    }
    private void OnDisable()
    {
        inputActions.Control.LeftD.performed -= _ => Emit(0);
        inputActions.Control.LeftF.performed -= _ => Emit(1);
        inputActions.Control.RightJ.performed -= _ => Emit(2);
        inputActions.Control.RightK.performed -= _ => Emit(3);
        inputActions.Control.Disable();
    }
    void Emit(int lane)
    {
        OnPlayerHit?.Invoke(lane);
    }
}
