using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {
    public static EnemyManager Instance;
    public Transform spawnPoint;
    public StatsData activeEnemyHp; 
    
    public List<EnemyTemplate> levelEnemies; 
    private int currentEnemyIndex = 0;
    private GameObject currentEnemyInstance;

    private void Awake() => Instance = this;

    void Start() {
        SpawnNextEnemy();
    }

    public void SpawnNextEnemy() {
        if (currentEnemyIndex >= levelEnemies.Count) {
            Debug.Log("จบด่าน! ชนะแล้ว");
            return;
        }

        if (currentEnemyInstance != null) Destroy(currentEnemyInstance);

        EnemyTemplate template = levelEnemies[currentEnemyIndex];
        
        activeEnemyHp.maxHp = template.maxHp;
        activeEnemyHp.ResetHp();

        currentEnemyInstance = Instantiate(template.visualPrefab, spawnPoint.position, spawnPoint.rotation);
        
        var visuals = currentEnemyInstance.GetComponent<CharacterVisuals>();
        if (visuals != null) visuals.isPlayer = false;

        currentEnemyIndex++;
    }
}