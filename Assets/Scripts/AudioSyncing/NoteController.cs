using UnityEngine;

public class NoteController : MonoBehaviour
{
    private NoteInfo noteData;
    private float spawnBeat;
    private float targetBeat;
    
    // Config values (Can be moved to a ScriptableObject later for better architecture)
    private Vector3 spawnPos;
    private Vector3 removePos;
    private Vector3 hitPos;
    
    private float beatsShownBeforeHit = 2.0f; // How many beats it takes to reach the hit zone

    public void Initialize(NoteInfo info, Vector3 startPos, Vector3 endPos, float beatsBefore)
    {
        noteData = info;
        targetBeat = info.beat;
        spawnPos = startPos;
        hitPos = endPos;
        beatsShownBeforeHit = beatsBefore;
        
        // Calculate where it should disappear (optional: for past the hit zone)
        removePos = hitPos + (hitPos - spawnPos); 
    }

    void Update()
    {
        if (Conductor.Instance == null) return;

        float currentBeat = Conductor.Instance.songPositionInBeats;
        
        // Calculate T (0 to 1) based on current beat relative to target beat
        // T = 0 at (targetBeat - beatsShownBeforeHit)
        // T = 1 at targetBeat
        float t = 1f - ((targetBeat - currentBeat) / beatsShownBeforeHit);

        if (t >= 0)
        {
            // Move from spawn to hit position
            transform.position = Vector3.Lerp(spawnPos, hitPos, t);
        }

        // Logic to remove note if it's too far past
        if (t > 1.2f) 
        {
            RhythmEvents.TriggerNoteMiss(); // Observer Pattern: Notify that we missed
            Destroy(gameObject);
        }
    }
}