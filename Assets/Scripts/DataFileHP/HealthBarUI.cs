using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour {
    public Slider hpSlider;
    public StatsData stats;

    void OnEnable() {
        HealthEvents.OnHpChanged += UpdateUI;
        UpdateUI();
    }

    void OnDisable() => HealthEvents.OnHpChanged -= UpdateUI;

    void UpdateUI() {
        if (hpSlider != null && stats != null) {
            hpSlider.value = stats.currentHp / stats.maxHp;
        }
    }
}
