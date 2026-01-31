using System.Collections;
using UnityEngine;

public class HandleHitAnimation : MonoBehaviour
{
    [SerializeField] private Animator hitAnimator;
    [SerializeField] private Material targetMaterial;
    [SerializeField] private int laneIndex;
    [SerializeField] private float highlightDuration = 0.1f; // ระยะเวลาที่จะให้ไฟสว่าง

    private Material originalMaterial;
    private Renderer rendererComponent;
    private Coroutine materialCoroutine;

    void Start()
    {
        rendererComponent = GetComponent<Renderer>();
        // เก็บค่า Material เริ่มต้นไว้
        originalMaterial = rendererComponent.material;
    }

    void OnEnable()
    {
        ButtonController.OnPlayerHit += PlayHitAnimation;
    }

    void OnDisable()
    {
        ButtonController.OnPlayerHit -= OnPlayerHitCleanup;
    }

    private void OnPlayerHitCleanup(int lane) => ButtonController.OnPlayerHit -= PlayHitAnimation;

    private void PlayHitAnimation(int lane)
    {
        // 1. เช็คเลนก่อนเลย ถ้าไม่ใช่เลนนี้ ไม่ต้องทำอะไรต่อ
        if (lane != laneIndex) return;

        // 2. เล่น Animation
        if (hitAnimator != null)
        {
            hitAnimator.SetTrigger("Hit");
        }

        // 3. เริ่มการเปลี่ยน Material แบบมีหน่วงเวลา
        if (materialCoroutine != null) StopCoroutine(materialCoroutine);
        materialCoroutine = StartCoroutine(FlashMaterial());
    }

    private IEnumerator FlashMaterial()
    {
        // เปลี่ยนเป็น Material เป้าหมาย
        rendererComponent.material = targetMaterial;

        // รอตามเวลาที่กำหนด (unscaled เพื่อให้ทำงานได้แม้หยุดเกม)
        yield return new WaitForSecondsRealtime(highlightDuration);

        // เปลี่ยนกลับเป็น Material เดิม
        rendererComponent.material = originalMaterial;
        materialCoroutine = null;
    }
}