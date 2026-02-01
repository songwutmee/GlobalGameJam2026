using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ButtonController : MonoBehaviour
{
    private PlayerInputActions inputActions;
    public static event Action<int> OnPlayerHit;
    public static event Action OnPauseTriggered;

    private void Awake()
    {
        // สร้างระบบ Input แค่ครั้งเดียว
        if (inputActions == null)
            inputActions = new PlayerInputActions();
            
        HideCursor();
    }

    private void OnEnable()
    {
        if (inputActions == null) inputActions = new PlayerInputActions();
        
        inputActions.Control.Enable();

        // เชื่อมต่อปุ่มกด (Manual Subscription ดีที่สุดสำหรับเกม Rhythm)
        inputActions.Control.LeftD.performed += HandleLeftD;
        inputActions.Control.LeftF.performed += HandleLeftF;
        inputActions.Control.RightJ.performed += HandleRightJ;
        inputActions.Control.RightK.performed += HandleRightK;
        inputActions.Control.Pause.performed += HandleEscape;
    }

    private void OnDisable()
    {
        if (inputActions != null)
        {
            inputActions.Control.LeftD.performed -= HandleLeftD;
            inputActions.Control.LeftF.performed -= HandleLeftF;
            inputActions.Control.RightJ.performed -= HandleRightJ;
            inputActions.Control.RightK.performed -= HandleRightK;
            inputActions.Control.Pause.performed -= HandleEscape;

            inputActions.Control.Disable();
        }
    }

    private void OnDestroy()
    {
        // ล้าง Static Event เพื่อป้องกัน Memory Leak ตอนเปลี่ยนฉาก
        OnPlayerHit = null;
        OnPauseTriggered = null;
        
        if (inputActions != null)
        {
            inputActions.Dispose();
            inputActions = null;
        }
    }

    // เปลี่ยนชื่อเป็น Handle... และตั้งเป็น public เพื่อความชัวร์
    public void HandleLeftD(InputAction.CallbackContext ctx) => Emit(0);
    public void HandleLeftF(InputAction.CallbackContext ctx) => Emit(1);
    public void HandleRightJ(InputAction.CallbackContext ctx) => Emit(2);
    public void HandleRightK(InputAction.CallbackContext ctx) => Emit(3);
    public void HandleEscape(InputAction.CallbackContext ctx) => OnPauseTriggered?.Invoke();

    private void Emit(int lane)
    {
        if (Conductor.Instance == null) return;
        OnPlayerHit?.Invoke(lane);
    }

    public void HideCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}