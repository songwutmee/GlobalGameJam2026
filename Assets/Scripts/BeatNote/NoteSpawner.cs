using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct NotePrefabData
{
    public GameObject prefab;
    [Range(0f, 100f)]
    public float percentageOfAppearing;
}

public class NoteSpawner : MonoBehaviour
{
    [Header("References")]
    public SongData songData;
    public NotePrefabData[] notePrefab;
    public Transform[] laneSpawnPoints;
    public Transform[] laneHitPoints;
    [SerializeField] private AudioSource musicSource;

    [Header("Settings")]
    [SerializeField] private int maxBombsPerMoment = 1;
    public float noteTravelTime = 2f; // ความเร็วเริ่มต้น
    
    private float currentTravelTime; // ความเร็วที่ปรับตาม Phase
    private int nextIndex = 0;
    private bool musicStarted = false;
    private float dspSongStartTime;
    private int[] weightTable = new int[100];
    private float lastSpawnTime = -1f;
    private int bombsSpawnedThisMoment = 0;

    void Awake()
    {
        // รีเซ็ตค่าเริ่มต้นทุกครั้งที่โหลดฉาก
        nextIndex = 0;
        musicStarted = false;
        currentTravelTime = noteTravelTime;
    }

    void Start()
    {
        GenerateLookupTable();
        
        // ตรวจสอบความพร้อมของระบบ
        if (Conductor.Instance != null)
        {
            StartSong();
        }
        else
        {
            Debug.LogError("Conductor Instance not found! Make sure Conductor is in the scene.");
        }
    }

    private void OnEnable()
    {
        BossPhaseManager.OnPhaseChanged += HandlePhaseChange;
    }

    private void OnDisable()
    {
        BossPhaseManager.OnPhaseChanged -= HandlePhaseChange;
    }

    public void StartSong()
    {
        Debug.Log("<color=green>[Spawner]</color> Starting Song...");
        if (songData == null || musicSource == null) return;
        Debug.Log("<color=green>[Spawner]</color> Song Data and Music Source are valid.");
        // ตั้งค่าเพลงจาก SongData
        if (songData.musicClip != null) musicSource.clip = songData.musicClip;
        Debug.Log("<color=green>[Spawner]</color> Music Clip assigned: " + musicSource.clip.name);

        // แก้ไขจุดตาย: บันทึก DSP Time ปัจจุบัน + 1 วินาที เพื่อเริ่มนับหนึ่งใหม่ในฉากนี้
        dspSongStartTime = (float)AudioSettings.dspTime + 1.0f;
        musicSource.PlayScheduled(dspSongStartTime);

        // ส่งค่า Baseline ไปให้ Conductor เพื่อ Reset เวลาให้ตรงกัน 100%
        if (Conductor.Instance != null)
        {
            Conductor.Instance.SetBaseline(dspSongStartTime);
        }
        Debug.Log("<color=green>[Spawner]</color> Baseline set in Conductor at DSP Time: " + dspSongStartTime);

        musicStarted = true;
        nextIndex = 0; 
        Debug.Log($"<color=green>[Spawner]</color> Song Started: {musicSource.clip.name} at DSP Time: {dspSongStartTime}");
    }

    public void StopSpawner()
    {
        musicStarted = false;
        if (musicSource != null) musicSource.Stop();
        ClearAllActiveNotes();
    }

    public void RestartSpawnerWithNewData()
    {
        // ใช้ฟังก์ชันนี้เมื่อเปลี่ยน Phase หรือเปลี่ยนเพลง
        StopSpawner();
        StartSong();
    }

    private void ClearAllActiveNotes()
    {
        // ล้างโน้ตทั้งหมดที่ค้างอยู่ในฉาก
        Notebase[] notes = FindObjectsOfType<Notebase>();
        foreach (Notebase n in notes) Destroy(n.gameObject);
    }

    private void HandlePhaseChange(int phase)
    {
        // ปรับความเร็วตาม Phase
        currentTravelTime = noteTravelTime - (phase - 1) * 0.4f;
        currentTravelTime = Mathf.Max(currentTravelTime, 0.8f);

        // ปรับความน่าจะเป็นของระเบิด
        if (phase == 2) SetNoteProbability<NoteBomb>(20f);
        else if (phase == 3) SetNoteProbability<NoteBomb>(40f);
        
        Debug.Log($"<color=yellow>[Spawner]</color> Phase {phase} - Speed Adjusted to: {currentTravelTime}");
    }

    void Update()
    {
        if (!musicStarted || Conductor.Instance == null || songData == null) return;

        float songTime = Conductor.Instance.songPositionSeconds;

        // ตรวจสอบจังหวะและ Spawn โน้ต
        while (nextIndex < songData.notes.Count &&
               songData.notes[nextIndex].timeInSeconds <= songTime + currentTravelTime)
        {
            Spawn(songData.notes[nextIndex]);
            nextIndex++;
        }
    }

    void Spawn(NoteInfo info)
    {
        if (info.laneIndex < 0 || info.laneIndex >= laneSpawnPoints.Length) return;

        // รีเซ็ตการนับระเบิดถ้าเป็นจังหวะใหม่
        if (!Mathf.Approximately(info.timeInSeconds, lastSpawnTime))
        {
            lastSpawnTime = info.timeInSeconds;
            bombsSpawnedThisMoment = 0;
        }

        Transform spawnPoint = laneSpawnPoints[info.laneIndex];
        Transform hitPoint = laneHitPoints[info.laneIndex];

        // สุ่มเลือก Prefab ตามน้ำหนักที่ตั้งไว้
        GameObject noteObject = GetWeightedNotePrefab();

        // จำกัดจำนวนระเบิดในหนึ่งจังหวะ
        if (noteObject.GetComponent<NoteBomb>() != null && bombsSpawnedThisMoment >= maxBombsPerMoment)
        {
            noteObject = GetNonBombPrefab();
        }

        if (noteObject.GetComponent<NoteBomb>() != null)
            bombsSpawnedThisMoment++;

        // สร้างโน้ตในฉาก
        GameObject note = Instantiate(noteObject, spawnPoint.position, Quaternion.Euler(0, 90, 0));

        // ส่งข้อมูลให้โน้ต (ใช้ currentTravelTime เพื่อให้ความเร็วถูกต้อง)
        Notebase noteScript = note.GetComponent<Notebase>();
        if (noteScript != null)
        {
            noteScript.Initialize(info.timeInSeconds, info.laneIndex, spawnPoint, hitPoint, currentTravelTime);
        }
    }

    // --- ระบบสุ่มแบบ Weight Probability ---

    GameObject GetWeightedNotePrefab()
    {
        return notePrefab[weightTable[UnityEngine.Random.Range(0, 100)]].prefab;
    }

    GameObject GetNonBombPrefab()
    {
        List<GameObject> safeNotes = new List<GameObject>();
        foreach (var n in notePrefab)
        {
            if (n.prefab.GetComponent<NoteBomb>() == null)
                safeNotes.Add(n.prefab);
        }
        return safeNotes[UnityEngine.Random.Range(0, safeNotes.Count)];
    }

    void GenerateLookupTable()
    {
        int currentSlot = 0;
        for (int i = 0; i < notePrefab.Length; i++)
        {
            int slotsToFill = Mathf.RoundToInt(notePrefab[i].percentageOfAppearing);
            for (int j = 0; j < slotsToFill && currentSlot < 100; j++)
            {
                weightTable[currentSlot] = i;
                currentSlot++;
            }
        }
        while (currentSlot < 100)
        {
            weightTable[currentSlot] = 0;
            currentSlot++;
        }
    }

    private void SetNoteProbability<T>(float targetPercent) where T : MonoBehaviour
    {
        int targetIndex = -1;
        for (int i = 0; i < notePrefab.Length; i++)
        {
            if (notePrefab[i].prefab.GetComponent<T>() != null)
            {
                targetIndex = i;
                break;
            }
        }

        if (targetIndex == -1) return;

        notePrefab[targetIndex].percentageOfAppearing = targetPercent;
        NormalizeOtherNotes(targetIndex, targetPercent);
        GenerateLookupTable();
    }

    private void NormalizeOtherNotes(int fixedIndex, float fixedPercent)
    {
        float remainingPercent = 100f - fixedPercent;
        float currentOtherTotal = 0f;
        for (int i = 0; i < notePrefab.Length; i++)
        {
            if (i != fixedIndex) currentOtherTotal += notePrefab[i].percentageOfAppearing;
        }
        for (int i = 0; i < notePrefab.Length; i++)
        {
            if (i != fixedIndex && currentOtherTotal > 0)
            {
                notePrefab[i].percentageOfAppearing = (notePrefab[i].percentageOfAppearing / currentOtherTotal) * remainingPercent;
            }
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (notePrefab == null || notePrefab.Length == 0) return;
        float total = 0;
        foreach (var note in notePrefab) total += note.percentageOfAppearing;
        if (total > 0)
        {
            for (int i = 0; i < notePrefab.Length; i++)
                notePrefab[i].percentageOfAppearing = (notePrefab[i].percentageOfAppearing / total) * 100f;
        }
    }
#endif
}