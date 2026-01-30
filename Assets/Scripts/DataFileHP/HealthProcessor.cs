using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthProcessor : MonoBehaviour {
    [Header("Data References")]
    public StatsData playerStats;
    public StatsData enemyStats;

    [Header("Settings")]
    public float damageToEnemy = 10f;
    public float damageToPlayer = 5f;
    public float healAmount = 2f;

    void OnEnable() 
    {
        RhythmEvents.OnNoteHit += HandleNoteHit;
        RhythmEvents.OnNoteMiss += HandleNoteMiss;
        playerStats.ResetHp();
        enemyStats.ResetHp();
    }

    void OnDisable() 
    {
        RhythmEvents.OnNoteHit -= HandleNoteHit;
        RhythmEvents.OnNoteMiss -= HandleNoteMiss;
    }

    private void HandleNoteHit() {
    enemyStats.ChangeHp(-damageToEnemy);
    
    if (enemyStats.currentHp <= 0) {
        // ศัตรูตาย!
        //RhythmEvents.TriggerEnemyDeath(); // แจ้งเตือนว่าตายแล้ว
        EnemyManager.Instance.SpawnNextEnemy(); // เรียกตัวถัดไป
    }
}

    private void HandleNoteMiss() 
    {
        playerStats.ChangeHp(-damageToPlayer);
    }
    
}