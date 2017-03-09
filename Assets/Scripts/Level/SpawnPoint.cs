using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour {

    public enum enemyType
    {
        nothing,
        runner,
        ranged
    }

    public enemyType[] toSpawn;
	public float[] spawnDelay;
    private int waveCount;

	// Use this for initialization
	void Start () {
        waveCount = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void spawn() {
		if (waveCount >= toSpawn.Length)
			return;
		StartCoroutine ("spawnWithDelay");
    }

	IEnumerator spawnWithDelay(){
		GameObject temp;
		if (waveCount < spawnDelay.Length)
			yield return new WaitForSeconds (spawnDelay [waveCount]);
		Debug.Log(gameObject.name + "spawn " + toSpawn[waveCount] + " on " + transform.position.x+" "+ transform.position.y+" "+ transform.position.z);
		switch (toSpawn[waveCount])
		{
		case enemyType.nothing:
			break;
		case enemyType.runner:
			temp = (GameObject) Instantiate(Resources.Load("runner"), transform.position, transform.rotation);
			break;
		case enemyType.ranged:
			temp = (GameObject) Instantiate (Resources.Load ("ranged"), transform.position, transform.rotation);
			break;
		default:
			break;
		}

		waveCount++;
	}
}
