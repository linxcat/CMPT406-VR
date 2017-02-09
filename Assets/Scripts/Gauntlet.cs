using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gauntlet : MonoBehaviour {

    private float validSpeed = 10;
	private float[] pastSpeed;
	private int counter;
	private float totalSpeed, avgSpeed;

	// Use this for initialization
	void Start () {
		pastSpeed = new float[10];
		counter = 0;
		for(int i = 0; i < 10; i++){
			pastSpeed [i] = 0;
		}
	}
	
	// Update is called once per frame
	void Update () {
		pastSpeed [counter++] = this.GetComponent<Rigidbody> ().velocity.magnitude;
		if (counter == 10)
			counter = 0;
		totalSpeed = 0;
		for(int i = 0; i < 10; i++){
			totalSpeed += pastSpeed [i];
		}
		if (totalSpeed == 0)
			avgSpeed = 0;
		avgSpeed = totalSpeed / 10;
	}

    private void OnTriggerEnter(Collider other)
	{				
		Debug.Log("Gauntlet speed check with: " + avgSpeed);
		if (speedCheck (validSpeed)) {
			//check tag
			if (other.tag == "Weapon") {
				//Call enemy's counter function
				//EnemyScript enemyscript = other.gameObject.GetComponent<EnemyScript>();
				//enemyscript.counter();
				Debug.Log("Gauntlet counter weapon");
			} else if (other.tag == "projectile") {
				//call projectile's counter function
				//ProjectileScript projectilescript = other.gameObject.GetComponent<ProjectileScript>();
				//projectilescript.counter();
				Debug.Log("Gauntlet counter projectile");
			} else if (other.tag == "Enemy") {
				//Punch enemy
				//EnemyScript enemyscript = other.gameObject.GetComponent<EnemyScript>();
				//enemyscript.punch();
				Debug.Log("Gauntlet punch enemy");
			}
		}
    }

    private bool speedCheck(float s)
    {
		return avgSpeed > s;
    }
}
