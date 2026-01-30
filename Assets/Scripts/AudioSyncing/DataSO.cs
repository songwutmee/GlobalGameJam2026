using System.Collections;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct NoteInfo {
    public float beat;      // Beat number (e.g., 1.5, 2.0)
    public int laneIndex;   // 0: Left, 1: Up, 2: Down, 3: Right
}

[CreateAssetMenu(fileName = "NewSong", menuName = "RhythmGame/SongData")]
public class SongData : ScriptableObject {
    public AudioClip musicClip;
    public float bpm;
    public float firstBeatOffset; // Offset in seconds before music starts
    public List<NoteInfo> notes;
}
