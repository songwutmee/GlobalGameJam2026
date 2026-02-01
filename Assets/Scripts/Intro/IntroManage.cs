using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private List<IntroStep> introSteps;
    [SerializeField] private float fadeSpeed = 1.5f;
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private string nextSceneName = "MainGame";

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI dialogueTextMesh;
    [SerializeField] private GameObject continuePrompt;

    private int currentIndex = 0;
    private bool isProcessing = false; // กันคนกดรัวขณะกำลัง Fade หรือ Type
    private string currentFullText;

    void Start()
    {
        // Setup เริ่มต้น: ซ่อนทุกอย่าง
        dialogueTextMesh.text = "";
        if (continuePrompt != null) continuePrompt.SetActive(false);

        foreach (var step in introSteps)
        {
            if (step.maskCanvasGroup != null)
            {
                step.maskCanvasGroup.alpha = 0f;
            }
        }

        // เริ่มขั้นตอนแรก
        StartCoroutine(PlayNextStep());
    }

    void Update()
    {
        // กดเพื่อไปต่อ
        if ((Input.anyKeyDown || Input.GetMouseButtonDown(0)) && !isProcessing)
        {
            currentIndex++;
            if (currentIndex < introSteps.Count)
            {
                StartCoroutine(PlayNextStep());
            }
            else
            {
                LoadMainGame();
            }
        }
    }

    private IEnumerator PlayNextStep()
    {
        isProcessing = true; // ล็อคปุ่มกดชั่วคราวขณะ Animation ทำงาน
        if (continuePrompt != null) continuePrompt.SetActive(false);
        
        IntroStep currentStep = introSteps[currentIndex];
        dialogueTextMesh.text = "";

        // 1. FADE หน้ากากขึ้นมาก่อน
        if (currentStep.maskCanvasGroup != null)
        {
            float alpha = 0;
            while (alpha < 1f)
            {
                alpha += Time.deltaTime * fadeSpeed;
                currentStep.maskCanvasGroup.alpha = alpha;
                yield return null;
            }
            currentStep.maskCanvasGroup.alpha = 1f;
        }

        // รอจังหวะนิดนึงหลังหน้ากากขึ้นจบ (0.3 วิ) เพื่อความสวยงาม
        yield return new WaitForSeconds(0.3f);

        // 2. เริ่มพิมพ์ DIALOGUE (Typewriter)
        currentFullText = currentStep.dialogueText;
        foreach (char c in currentFullText.ToCharArray())
        {
            dialogueTextMesh.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isProcessing = false; // ปลดล็อคให้กดไปต่อได้
        if (continuePrompt != null) continuePrompt.SetActive(true);
    }

    public void LoadMainGame()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}