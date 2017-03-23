using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum enemyType
{
    nothing,
    runner,
    walker,
    ranged
}

[System.Serializable]
public class Wave
{
    public enemyType[] enemy;
    public float delay;
}

public class SpawnPoint : MonoBehaviour {

    public Wave[] waves;
    private int waveCount, enemyCount;

	// Use this for initialization
	void Start () {
        waveCount = 0;
        enemyCount = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void spawn() {
		if (waveCount >= waves.Length)
			return;
		StartCoroutine ("spawnWithDelay");
    }

	IEnumerator spawnWithDelay(){
		GameObject temp;
        enemyCount = 0;
        while (enemyCount < waves[waveCount].enemy.Length) {
            yield return new WaitForSeconds(waves[waveCount].delay);
            Debug.Log(gameObject.name + "spawn " + waves[waveCount] + " on " + transform.position.x + " " + transform.position.y + " " + transform.position.z);
            switch (waves[waveCount].enemy[enemyCount])
            {
                case enemyType.nothing:
                    break;
                case enemyType.runner:
                    temp = (GameObject)Instantiate(Resources.Load("runner"), transform.position, transform.rotation);
                    SendMessageUpwards("EnemySpawned");
                    break;
                case enemyType.walker: //TODO put walker here
                    temp = (GameObject)Instantiate(Resources.Load("runner"), transform.position, transform.rotation);
                    SendMessageUpwards("EnemySpawned");
                    break;
                case enemyType.ranged:
                    temp = (GameObject)Instantiate(Resources.Load("ranged"), transform.position, transform.rotation);
                    SendMessageUpwards("EnemySpawned");
                    break;
                default:
                    break;
            }
            enemyCount++;
        }
		waveCount++;
	}
}
