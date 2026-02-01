using UnityEngine;

public class Conductor : MonoBehaviour
{
    public static Conductor Instance;
    public SongData songData;
    public AudioSource musicSource;

    public float songPositionSeconds;
    public float dspSongTime;

    private double pauseStartTime;
    private double totalPausedTime;
    public bool isPaused = false;

    private void Awake()
    {
        // บังคับให้เป็นตัวปัจจุบันเสมอ
        Debug.Log("Conductor Awake called.");
        Instance = this;
        Time.timeScale = 1f;
        totalPausedTime = 0;
    }

    // ฟังก์ชันสำหรับ Spawner เรียกใช้เพื่อ Reset เวลาให้ตรงกัน
    public void SetBaseline(float startTime)
    {
        dspSongTime = startTime;
        totalPausedTime = 0;
        songPositionSeconds = 0;
        isPaused = false;
    }

    public void PauseSong()
    {
        isPaused = true;
        pauseStartTime = AudioSettings.dspTime;
        musicSource.Pause();
    }

    public void ResumeSong()
    {
        isPaused = false;
        totalPausedTime += (AudioSettings.dspTime - pauseStartTime);
        musicSource.UnPause();
    }

    void Update()
    {
        if (musicSource != null && musicSource.isPlaying && !isPaused)
        {
            Debug.Log("Conductor Update calculating song position.");
            // คำนวณเวลาที่แม่นยำที่สุด
            songPositionSeconds = (float)(AudioSettings.dspTime - dspSongTime - totalPausedTime) - songData.firstBeatOffset;
        }
    }
}