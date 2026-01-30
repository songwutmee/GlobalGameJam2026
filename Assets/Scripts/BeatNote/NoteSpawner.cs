using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    public SongData songData;
    public GameObject notePrefab;
    public Transform[] laneSpawnPoints;
    public Transform[] laneHitPoints;
    public float noteTravelTime = 2f;

    private int nextIndex = 0;

    void Update()
    {
        float songTime = Conductor.Instance.songPositionSeconds;

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

        note.GetComponent<NoteObject>()
            .Initialize(info.timeInSeconds, info.laneIndex, spawnPoint, hitPoint, noteTravelTime);
    }
}
