using UnityEngine;
using System;

public class RhythmEvents : MonoBehaviour {
    // Observer Pattern: Events for other systems to subscribe to
    public static event Action OnNoteHit;
    public static event Action OnNoteMiss;
    public static event Action<int> OnBeatUpdate; // Pass current beat count

    public static void TriggerNoteHit() => OnNoteHit?.Invoke();
    public static void TriggerNoteMiss() => OnNoteMiss?.Invoke();
}
