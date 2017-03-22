using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour {

    public int numberOfWaves = 3;
    public int[] enemiesToNextWave;
    public int totalEnemies;
    public float[] timeInterval;
    private float timeCount;
    private string spawnMesasge = "spawn";
    private int enemiesLive, enemiesKilled;

	// Use this for initialization
	void Start () {
        timeCount = 0;
	}
	
	// Update is called once per frame
	void Update () {
        timeCount += Time.deltaTime;
        if (numberOfWaves > 0 && numberOfWaves < timeInterval.Length && timeCount > timeInterval[numberOfWaves]) {
            if (numberOfWaves < enemiesToNextWave.Length && enemiesLive <= enemiesToNextWave[numberOfWaves]) {
                BroadcastMessage(spawnMesasge);
                timeCount = 0;
                numberOfWaves--;
            }
        }
	}

    public void EnemySpawned() {
        enemiesLive++;
    }

    public void EnemyKilled() {
        enemiesLive--;
        enemiesKilled++;
    }
}
