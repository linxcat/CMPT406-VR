using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour {

    public int numberOfWaves = 3;
    private int currentWave;
    public int totalEnemies;
    public int[] enemiesToNextWave;
    public float[] timeInterval;
    private float timeCount;
    private string spawnMesasge = "spawn";
    private int enemiesLive, enemiesKilled;
    private LevelManager levelManager;

	// Use this for initialization
	void Start () {
        timeCount = 0;
        currentWave = 0;
        levelManager = FindObjectOfType<LevelManager>();
	}
	
	// Update is called once per frame
	void Update () {
        timeCount += Time.deltaTime;
        if (currentWave < numberOfWaves && currentWave < timeInterval.Length && timeCount > timeInterval[currentWave]) {
            if (currentWave < enemiesToNextWave.Length && enemiesLive <= enemiesToNextWave[currentWave]) {
                BroadcastMessage(spawnMesasge);
                timeCount = 0;
                currentWave++;
            }
        }
        if (enemiesKilled >= totalEnemies)
        {
            levelManager.gameWon();
        }

	}

    public void EnemySpawned() {
        enemiesLive++;
    }

    public void EnemyKilled() {
        enemiesLive--;
        enemiesKilled++;
    }

    public int CurrentWave(){
        return currentWave;
    }
}
