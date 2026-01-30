using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteObject : MonoBehaviour
{
    [SerializeField] private float delayBeforeDestroy = 0.2f;
    [SerializeField] private float perfectHitAcceptableThreshold = 0.08f;
    [SerializeField] private float hitAcceptableThreshold = 0.15f;
    public static event Action<int> OnNoteHit;
    public static event Action<int> OnNoteHitPerfect;
    public static event Action<int> OnNoteMiss;
    private Transform spawnPoint;
    private Transform hitPoint;
    private float travelTime;
    private float hitTime;
    private int laneIndex;

    public void Initialize(float targetTime, int lane, Transform spawn, Transform hit, float travel)
    {
        hitTime = targetTime;
        laneIndex = lane;
        spawnPoint = spawn;
        hitPoint = hit;
        travelTime = travel;
    }

    void OnEnable()
    {
        ButtonController.OnPlayerHit += HandlePlayerHit;
    }

    void OnDisable()
    {
        ButtonController.OnPlayerHit -= HandlePlayerHit;
    }

    void HandlePlayerHit(int inputLane)
    {
        if (inputLane != laneIndex)
            return;
        float songTime = Conductor.Instance.songPositionSeconds;
        float offset = songTime - hitTime;
        float absOffset = Mathf.Abs(songTime - hitTime);

        // Debug.Log($"[NOTE CHECK] Lane: {laneIndex} | Offset: {offset:F4}s | AbsOffset: {absOffset:F4}s");

        if (absOffset < 0.25f) 
        {
            if (absOffset < 0.12f) OnNoteHitPerfect?.Invoke(laneIndex);
            else OnNoteHit?.Invoke(laneIndex);

            Destroy(gameObject);
        }
    }

    void Update()
    {
        float songTime = Conductor.Instance.songPositionSeconds;

        float timeUntilHit = hitTime - songTime;
        float progress = 1f - (timeUntilHit / travelTime);
        progress = Mathf.Clamp01(progress);

        transform.position = Vector3.Lerp(spawnPoint.position, hitPoint.position, progress);

        if (songTime - hitTime > 0.15f)
        {
            OnNoteMiss?.Invoke(laneIndex);
            Destroy(gameObject, delayBeforeDestroy);
        }
    }

    public bool TryHit()
    {
        float songTime = Conductor.Instance.songPositionSeconds;
        float offset = songTime - hitTime;
        float absOffset = Mathf.Abs(songTime - hitTime);

        // Debug.Log($"[NOTE CHECK] Lane: {laneIndex} | Offset: {offset:F4}s | AbsOffset: {absOffset:F4}s");

        if (absOffset < hitAcceptableThreshold) 
        {
            if (absOffset < perfectHitAcceptableThreshold) OnNoteHitPerfect?.Invoke(laneIndex);
            else OnNoteHit?.Invoke(laneIndex);

            Destroy(gameObject);
            return true; 
        }
        return false;
    }
}
