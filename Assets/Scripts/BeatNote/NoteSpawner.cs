using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    [SerializeField] private float groundOffset = 1f;
    public SongData songData;
    public GameObject notePrefab;
    public Transform[] laneSpawnPoints;
    public Transform[] laneHitPoints;
    public float noteTravelTime = 2f;
    private bool musicStarted = false;
    private double dspSongStartTime;
    [SerializeField] private AudioSource musicSource;

    void Start()
    {
        StartSong();
    }

    public void StartSong()
    {
        dspSongStartTime = AudioSettings.dspTime + 1.0f; 
        musicSource.PlayScheduled(dspSongStartTime);
        musicStarted = true;
    }

    private int nextIndex = 0;

    void Update()
    {
        float songTime = Conductor.Instance.songPositionSeconds;
        if (!musicStarted)
        {
            Conductor.Instance.songPositionSeconds = 0f;
            return;
        }

        if (songTime > 1000) Debug.LogError("Song Time is HUGE: " + songTime);

        while (nextIndex < songData.notes.Count &&
               songData.notes[nextIndex].timeInSeconds <= songTime + noteTravelTime)
        {
            Spawn(songData.notes[nextIndex]);
            nextIndex++;
        }
    }

    void Spawn(NoteInfo info)
    {
        Transform spawnPoint = laneSpawnPoints[info.laneIndex];
        Transform hitPoint = laneHitPoints[info.laneIndex];

        GameObject note = Instantiate(notePrefab, spawnPoint.position, Quaternion.identity);

        NoteObject noteScript = note.GetComponent<NoteObject>();
        noteScript.Initialize(info.timeInSeconds, info.laneIndex, spawnPoint, hitPoint, noteTravelTime);
    }
}
