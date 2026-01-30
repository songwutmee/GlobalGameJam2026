using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicConductor : MonoBehaviour
{
    public static MusicConductor Instance;
    public AudioSource audioSource;

    public float SongTime => audioSource.time;

    void Awake()
    {
        Instance = this;
    }
}
