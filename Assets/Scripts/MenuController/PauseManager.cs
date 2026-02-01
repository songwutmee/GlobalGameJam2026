using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

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
        // ฟัง Event จากระบบ Input ใหม่
        ButtonController.OnPauseTriggered += TogglePause;
        BattleManager.OnGameStateChanged += HandleGameStateChanged;
    }

    void OnDisable()
    {
        ButtonController.OnPauseTriggered -= TogglePause;
        BattleManager.OnGameStateChanged -= HandleGameStateChanged;
    }

    void Awake()
    {
        canvasGroup = pausePanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = pausePanel.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        pausePanel.SetActive(false);
    }

    void TogglePause()
    {
        if (isTransitioning || gameEnded) return;
        if (isPaused) Resume();
        else Pause();
    }

    public void Pause()
    {
        if (isPaused) return;
        isPaused = true;
        Time.timeScale = 0f;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (Conductor.Instance != null) Conductor.Instance.PauseSong();

        pausePanel.SetActive(true);
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadePauseMenu(1f, true));
    }

    public void Resume()
    {
        if (!isPaused || isTransitioning) return;
        StartCoroutine(ResumeCountdown());
    }

    private IEnumerator ResumeCountdown()
    {
        isTransitioning = true;
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        yield return StartCoroutine(FadePauseMenu(0f, false));

        countdownText.gameObject.SetActive(true);
        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSecondsRealtime(1f);
        }
        countdownText.text = "GO!";
        yield return new WaitForSecondsRealtime(0.5f);
        countdownText.gameObject.SetActive(false);

        isPaused = false;
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (Conductor.Instance != null) Conductor.Instance.ResumeSong();
        isTransitioning = false;
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private IEnumerator FadePauseMenu(float target, bool isOpening)
    {
        float start = canvasGroup.alpha;
        float time = 0;
        while (time < fadeDuration)
        {
            time += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, target, time / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = target;
        if (!isOpening) pausePanel.SetActive(false);
    }

    void HandleGameStateChanged(gameState state) { gameEnded = true; }
}