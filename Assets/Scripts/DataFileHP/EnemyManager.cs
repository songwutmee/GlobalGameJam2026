using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public List<EnemyTemplate> enemyList;
    private int currentIndex = 0;
    
    public Transform enemySpawnPoint;
    public CharacterStats enemyStatsAsset;
    public GameObject winUI;

    void Start() {
        SpawnNextEnemy();
    }

    public void SpawnNextEnemy() {
        if (currentIndex >= enemyList.Count) return;

        winUI.SetActive(false);

        foreach (Transform child in enemySpawnPoint) Destroy(child.gameObject);

        EnemyTemplate et = enemyList[currentIndex];
        enemyStatsAsset.maxHp = et.maxHp;
        enemyStatsAsset.Initialize();

        GameObject enemyObj = Instantiate(et.visualPrefab, enemySpawnPoint);
        enemyObj.AddComponent<CharacterAnimator>().isPlayer = false;

        currentIndex++;
    }
}