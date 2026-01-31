using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct PhaseTimedHide {
    public GameObject targetObject;
    public float hideAtSecond; 
}

[System.Serializable]
public struct UniquePhaseData {
    public string phaseName;
    public SongData phaseSongData;      
    public AudioClip phaseMusic;        
    public string animationTrigger;     
    public float showDuration;          
    
    [Header("Visual Phase Items")]
    public GameObject maskToShow; 
    public List<GameObject> objectsToHideImmediately; 
    public List<PhaseTimedHide> timedHidingActions; 
    public float timeBeforeCameraPan;
}

public class BossPhaseSequencer : MonoBehaviour
{
    // ตัวแปร Static สำหรับตรวจสอบสถานะการเล่น Cinematic (ใช้ Lock Animation)
    public static bool IsCinematicActive { get; private set; }

    [Header("Phase 1 Setup")]
    public List<GameObject> phase1HideImmediately;
    public GameObject phase1Mask;

    [Header("Phase Configuration (Phase 2 & 3)")]
    public List<UniquePhaseData> phaseSettings; 

    [Header("References")]
    public BossCameraController cameraController;
    public NoteSpawner noteSpawner;
    public AudioSource musicSource;
    public Animator bossAnimator;

    void Start() {
        IsCinematicActive = false;
        if (phase1Mask != null) phase1Mask.SetActive(true);
        foreach (var obj in phase1HideImmediately) if(obj != null) obj.SetActive(false);
    }

    private void OnEnable() => BossPhaseManager.OnPhaseChanged += StartPhaseTransition;
    private void OnDisable() => BossPhaseManager.OnPhaseChanged -= StartPhaseTransition;

    private void StartPhaseTransition(int phaseIndex) {
        int settingIndex = phaseIndex - 2; 
        if (settingIndex < 0 || settingIndex >= phaseSettings.Count) return;

        StartCoroutine(ExecuteTransition(phaseSettings[settingIndex], phaseIndex));
    }

    private IEnumerator ExecuteTransition(UniquePhaseData settings, int phaseNum) {
        // เริ่มต้นการ Lock
        IsCinematicActive = true; 
        Debug.Log($"<color=orange>[Sequencer]</color> Starting Phase {phaseNum} - Animation Locked");

        noteSpawner.StopSpawner(); 
        musicSource.Stop();
        
        if (settings.maskToShow != null) settings.maskToShow.SetActive(true);
        foreach (var obj in settings.objectsToHideImmediately) if(obj != null) obj.SetActive(false);

        // เล่น Animation เปลี่ยน Phase ทันที
        if (bossAnimator != null && !string.IsNullOrEmpty(settings.animationTrigger))
            bossAnimator.SetTrigger(settings.animationTrigger);

        // รอตามเวลาที่ตั้งไว้ก่อนเริ่มแพนกล้อง
        yield return new WaitForSeconds(settings.timeBeforeCameraPan);

        if (cameraController != null) cameraController.ShowBossCamera(phaseNum, settings.showDuration);

        float timer = 0;
        List<PhaseTimedHide> pendingActions = new List<PhaseTimedHide>(settings.timedHidingActions);

        while (timer < settings.showDuration) {
            timer += Time.unscaledDeltaTime;
            for (int i = pendingActions.Count - 1; i >= 0; i--) {
                if (timer >= pendingActions[i].hideAtSecond) {
                    if (pendingActions[i].targetObject != null) pendingActions[i].targetObject.SetActive(false);
                    pendingActions.RemoveAt(i);
                }
            }
            yield return null;
        }

        // รอช่วงพัก 1.5 วิ ก่อนเริ่มเกมต่อ
        yield return new WaitForSeconds(1.5f);

        if (settings.phaseSongData != null) {
            noteSpawner.songData = settings.phaseSongData;
            musicSource.clip = settings.phaseMusic;
            noteSpawner.RestartSpawnerWithNewData();
        }

        // ปลดล็อคเมื่อทุกอย่างเสร็จสิ้น
        IsCinematicActive = false;
        Debug.Log("<color=green>[Sequencer]</color> Transition Finished - Animation Unlocked");
    }
}