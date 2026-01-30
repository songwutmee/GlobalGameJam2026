using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NoteFactory : MonoBehaviour {
    [SerializeField] private GameObject notePrefab;
    [SerializeField] private Transform[] spawnPoints; // The top points
    [SerializeField] private Transform[] hitPoints;   // The bottom points (where the player presses)
    [SerializeField] private float beatsShownBeforeHit = 2.0f;
    
    private int nextIndex = 0;

    void Update() {
        if (Conductor.Instance == null || Conductor.Instance.songData == null) return;

        var notes = Conductor.Instance.songData.notes;

        if (nextIndex < notes.Count) {
            // Check if it's time to spawn (target beat minus the lead time)
            if (Conductor.Instance.songPositionInBeats >= notes[nextIndex].beat - beatsShownBeforeHit) {
                SpawnNote(notes[nextIndex]);
                nextIndex++;
            }
        }
    }

    void SpawnNote(NoteInfo info) {
        // Factory logic: Instantiate and Initialize
        GameObject noteObj = Instantiate(notePrefab);
        NoteController controller = noteObj.GetComponent<NoteController>();
        
        if (controller != null) {
            controller.Initialize(
                info, 
                spawnPoints[info.laneIndex].position, 
                hitPoints[info.laneIndex].position,
                beatsShownBeforeHit
            );
        }
    }
}
