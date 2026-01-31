using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; // เพิ่มเพื่อใช้โหลดฉาก

public class PauseManager : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI countdownText;
    public GameObject pausePanel;
    [SerializeField] private float fadeDuration = 0.3f;
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    private bool isPaused = false;
    private bool isTransitioning = false;

    private CanvasGroup canvasGroup;
    private Coroutine fadeCoroutine;

    private bool gameEnded = false;

    void OnEnable()
    {
        BattleManager.OnGameStateChanged += HandleGameStateChanged;
    }

    void OnDisable()
    {
        BattleManager.OnGameStateChanged -= HandleGameStateChanged;
    }

    void HandleGameStateChanged(gameState state)
    {
        gameEnded = true;

        if (isPaused)
        {
            StopAllCoroutines();
            Time.timeScale = 1f;
            pausePanel.SetActive(false);
            isPaused = false;
        }
    }

    void Awake()
    {
        if (gameEnded) return;
        canvasGroup = pausePanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = pausePanel.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;
        pausePanel.SetActive(false);
    }

    void Update()
    {
        if (isTransitioning || gameEnded) return;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) Resume();
            else Pause();
        }
    }

    public void Resume()
    {
        // if (isTransitioning || gameEnded || isPaused) return;
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);

        StartCoroutine(ResumeCountdown());
    }

    public void Pause()
    {
        // if (isTransitioning || gameEnded || !isPaused) return;
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);

        isPaused = true;
        Time.timeScale = 0f;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (Conductor.Instance != null && Conductor.Instance.musicSource != null)
            Conductor.Instance.PauseSong();

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
    private IEnumerator ResumeCountdown()
    {
        isTransitioning = true;

        // 1. Hide the Pause Menu visually first
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        yield return StartCoroutine(FadePauseMenu(0f, false));

        // 2. Start Countdown
        countdownText.gameObject.SetActive(true);
        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            // Realtime is required because timeScale is 0
            yield return new WaitForSecondsRealtime(1f);
        }

        countdownText.text = "GO!";
        yield return new WaitForSecondsRealtime(0.5f);
        countdownText.gameObject.SetActive(false);

        // 3. NOW we unfreeze everything at the exact same time
        isPaused = false;
        Time.timeScale = 1f;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Resume the song logic and audio
        if (Conductor.Instance != null)
            Conductor.Instance.ResumeSong();

        isTransitioning = false;
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