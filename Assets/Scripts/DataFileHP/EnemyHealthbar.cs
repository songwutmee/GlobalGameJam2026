using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthbar : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider[] phaseSliders; 
    
    [Header("References")]
    public CharacterStats stats;

    private int currentPhaseIndex = 0;

    private void OnEnable()
    {
        if (stats != null) 
        {
            stats.OnHpChanged += UpdateUI;
        }

        // Listen to phase changes to know which slider to "activate"
        BossPhaseManager.OnPhaseChanged += HandlePhaseChange;

        // Initialize UI
        InitializeSliders();
    }

    private void OnDisable()
    {
        if (stats != null) stats.OnHpChanged -= UpdateUI;
        BossPhaseManager.OnPhaseChanged -= HandlePhaseChange;
    }

    private void InitializeSliders()
    {
        // Set all sliders to full at the start
        foreach (Slider s in phaseSliders)
        {
            if (s != null)
            {
                s.minValue = 0;
                s.maxValue = stats.maxHp;
                s.value = stats.maxHp;
            }
        }
        currentPhaseIndex = 0;
    }

    private void HandlePhaseChange(int phase)
    {
        int newIndex = phase - 1;
        for (int i = 0; i < newIndex; i++)
        {
            if (i < phaseSliders.Length) phaseSliders[i].value = 0;
            phaseSliders[i].gameObject.SetActive(false);
        }

        currentPhaseIndex = newIndex;
        Debug.Log($"<color=green>UI Switch:</color> Now updating Slider {currentPhaseIndex}");
    }

    void UpdateUI(float cur, float max)
    {
        if (currentPhaseIndex >= 0 && currentPhaseIndex < phaseSliders.Length)
        {
            Slider activeSlider = phaseSliders[currentPhaseIndex];
            
            if (activeSlider != null)
            {
                activeSlider.maxValue = max; 
                activeSlider.value = cur; 
                // Debug.Log($"Slider {currentPhaseIndex} updated: {cur}/{max}");
            }
        }
    }
}
