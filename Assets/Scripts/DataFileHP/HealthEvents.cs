using System.Collections;
using System.Collections.Generic;
using System;

public static class HealthEvents {
    public static event Action OnHpChanged;
    public static void TriggerHpChanged() => OnHpChanged?.Invoke();
}
