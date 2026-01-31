using System;
using UnityEngine;

public class NoteObject : MonoBehaviour
{
    [SerializeField] private float delayBeforeDestroy = 0.2f;
    public static event Action<int> OnNoteHit;
    public static event Action<int> OnNoteHitPerfect;
    public static event Action<int> OnNoteMiss;

    private Transform spawnPoint;
    private Transform hitPoint;
    private float travelTime;
    private float hitTime;
    private int laneIndex;
    
    private bool isHandled = false; 

    public void Initialize(float targetTime, int lane, Transform spawn, Transform hit, float travel)
    {
        hitTime = targetTime;
        laneIndex = lane;
        spawnPoint = spawn;
        hitPoint = hit;
        travelTime = travel;
        isHandled = false;
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
        if (isHandled || inputLane != laneIndex) return;

        float songTime = Conductor.Instance.songPositionSeconds;
        float absOffset = Mathf.Abs(songTime - hitTime);

        if (absOffset < 0.25f) 
        {
            isHandled = true; 
            
            if (absOffset < 0.12f) OnNoteHitPerfect?.Invoke(laneIndex);
            else OnNoteHit?.Invoke(laneIndex);
            
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (isHandled || Conductor.Instance == null) return;

        float songTime = Conductor.Instance.songPositionSeconds;
        float timeUntilHit = hitTime - songTime;
        float progress = 1f - (timeUntilHit / travelTime);
        progress = Mathf.Clamp01(progress);

        transform.position = Vector3.Lerp(spawnPoint.position, hitPoint.position, progress);

        // (Miss)
        if (songTime - hitTime > 0.15f)
        {
            isHandled = true; 
            OnNoteMiss?.Invoke(laneIndex);
            
            if (TryGetComponent<Renderer>(out Renderer r)) r.enabled = false;
            Destroy(gameObject, delayBeforeDestroy);
        }
    }
}