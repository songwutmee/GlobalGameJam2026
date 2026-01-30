using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteObject : MonoBehaviour
{
    [SerializeField] private float delayBeforeDestroy = 0.2f;
    private bool canBePressed;
    public static event Action OnNoteHit;
    public static event Action OnNoteHitPerfect;
    public static event Action OnNoteMiss;
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

    void Update()
    {
        float songTime = Conductor.Instance.songPositionSeconds;

        float timeUntilHit = hitTime - songTime;
        float progress = 1f - (timeUntilHit / travelTime);
        progress = Mathf.Clamp01(progress);

        transform.position = Vector3.Lerp(spawnPoint.position, hitPoint.position, progress);

        if (songTime - hitTime > 0.15f)
        {
            OnNoteMiss?.Invoke();
            gameObject.SetActive(false);
            Destroy(gameObject, delayBeforeDestroy);
        }
    }
    void OnEnable()
    {
        ButtonController.OnPlayerHit += OnNoteEnterPlayer;
    }

    void OnDisable()
    {
        ButtonController.OnPlayerHit -= OnNoteEnterPlayer;
    }

    private void OnNoteEnterPlayer(int lane)
    {
        if (!canBePressed || lane != laneIndex)
            return;

        float offset = Mathf.Abs(Conductor.Instance.songPositionSeconds - hitTime);

        if (offset < 0.08f)
        {
            Debug.Log("Perfect");
            OnNoteHitPerfect?.Invoke();
        }
        else if (offset < 0.15f)
        {
            Debug.Log("Hit");
            OnNoteHit?.Invoke();
        }
        else
        {
            Debug.Log("Missing");
            OnNoteMiss?.Invoke();
        }

        gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Activator"))
        {
            canBePressed = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Activator"))
        {
            canBePressed = false;
        }
    }
}
