using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewStats", menuName = "RhythmGame/Stats")]
public class StatsData : ScriptableObject {
    public float maxHp = 100f;
    public float currentHp;

    public void ResetHp() => currentHp = maxHp;

    public void ChangeHp(float amount) {
        currentHp = Mathf.Clamp(currentHp + amount, 0, maxHp);
        // sub event for ui update
        HealthEvents.TriggerHpChanged();
    }
}