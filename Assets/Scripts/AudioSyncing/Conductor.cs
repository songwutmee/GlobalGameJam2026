using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conductor : MonoBehaviour {
    public SongData songData;
    public AudioSource musicSource;

    // Singleton for easy access (Architecture choice)
    public static Conductor Instance;

    public float songPosition;       // Current time in seconds
    public float songPositionInBeats; 
    public float secPerBeat;
    public float dspSongTime;        // Time when song started

    private void Awake() {
        Instance = this;
    }

    void Start() {
        secPerBeat = 60f / songData.bpm;
        dspSongTime = (float)AudioSettings.dspTime;
        
        musicSource.clip = songData.musicClip;
        musicSource.Play();
    }

    void Update() {
        // Calculate position relative to dspTime for perfect sync
        songPosition = (float)(AudioSettings.dspTime - dspSongTime) - songData.firstBeatOffset;
        songPositionInBeats = songPosition / secPerBeat;

        
    }
}