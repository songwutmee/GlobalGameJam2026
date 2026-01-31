using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; // เพิ่มเพื่อใช้โหลดฉาก

public class PauseManager : MonoBehaviour
{
    public GameObject pausePanel;
    [SerializeField] private float fadeDuration = 0.3f; 
    [SerializeField] private string mainMenuSceneName = "MainMenu"; // ตั้งชื่อฉากเมนูหลักที่นี่
    
    private CanvasGroup canvasGroup;
    private bool isPaused = false;
    private Coroutine fadeCoroutine;

    void Awake()
    {
        canvasGroup = pausePanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = pausePanel.AddComponent<CanvasGroup>();
        
        canvasGroup.alpha = 0f;
        pausePanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) Resume();
            else Pause();
        }
    }

    public void Resume()
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        
        isPaused = false;
        Time.timeScale = 1f; 

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (Conductor.Instance != null && Conductor.Instance.musicSource != null)
            Conductor.Instance.musicSource.UnPause();

        fadeCoroutine = StartCoroutine(FadePauseMenu(0f, false));
    }

    public void Pause()
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);

        isPaused = true;
        Time.timeScale = 0f; 

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (Conductor.Instance != null && Conductor.Instance.musicSource != null)
            Conductor.Instance.musicSource.Pause();

        pausePanel.SetActive(true);
        fadeCoroutine = StartCoroutine(FadePauseMenu(1f, true));
    }

    // --- ฟังก์ชันใหม่สำหรับปุ่ม Back to Main Menu ---
    public void BackToMainMenu()
    {
        // 1. คืนเวลาให้โลกปกติ (สำคัญมาก ถ้าไม่ทำเมนูหลักจะค้าง)
        Time.timeScale = 1f;

        // 2. ปลดล็อคเมาส์ให้กดเมนูได้
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // 3. หยุดเพลงทันที
        if (Conductor.Instance != null && Conductor.Instance.musicSource != null)
        {
            Conductor.Instance.musicSource.Stop();
        }

        // 4. โหลดฉากเมนู
        Debug.Log("Exiting to Main Menu...");
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private IEnumerator FadePauseMenu(float targetAlpha, bool isOpening)
    {
        float startAlpha = canvasGroup.alpha;
        float time = 0;

        while (time < fadeDuration)
        {
            time += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;

        if (!isOpening)
        {
            pausePanel.SetActive(false);
        }
    }
}