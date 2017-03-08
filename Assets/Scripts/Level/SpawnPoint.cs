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
    private int waveCount;

	// Use this for initialization
	void Start () {
        waveCount = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void spawn() {
        Debug.Log(gameObject.name + "spawn " + toSpawn[waveCount] + " on " + transform.position.x+" "+ transform.position.y+" "+ transform.position.z);
        switch (toSpawn[waveCount])
        {
            case enemyType.nothing:
                break;
            case enemyType.runner:
                Instantiate(Resources.Load("runner"), transform.position, transform.rotation);
                break;
            case enemyType.ranged:
                Instantiate(Resources.Load("ranged"), transform.position, transform.rotation);
                break;
            default:
                break;
        }

        waveCount++;
    }
}
