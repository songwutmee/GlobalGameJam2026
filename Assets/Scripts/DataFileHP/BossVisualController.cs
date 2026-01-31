using UnityEngine;

public class BossVisualController : MonoBehaviour
{
    [SerializeField] private Renderer bossRenderer;
    [SerializeField] private float scaleOffset = 1.1f;
    [SerializeField] private Material texPhase1;
    [SerializeField] private Material texPhase2;
    [SerializeField] private Material texPhase3;

    private void OnEnable() => BossPhaseManager.OnPhaseChanged += ChangeVisual;
    private void OnDisable() => BossPhaseManager.OnPhaseChanged -= ChangeVisual;

    private void ChangeVisual(int phase) {
        if (bossRenderer == null) return;
        
        Material targetMat = texPhase1;
        if (phase == 2) targetMat = texPhase2;
        if (phase == 3) targetMat = texPhase3;

        bossRenderer.material = targetMat;
        
        transform.localScale *= scaleOffset;
    }
}