using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour {

    public int numberOfWaves = 3;
    public float timeInterval = 10F;
    public float timeIntervalIncreasePerWave = 2F;
    private float timeCount;
    private string spawnMesasge = "spawn";

	// Use this for initialization
	void Start () {
        timeCount = 0;
	}
	
	// Update is called once per frame
	void Update () {
        timeCount += Time.deltaTime;
        if (numberOfWaves > 0 && timeCount > timeInterval)
        {
            BroadcastMessage(spawnMesasge);
            timeCount = 0;
            timeInterval += timeIntervalIncreasePerWave;
            numberOfWaves--;
        }
	}
}
