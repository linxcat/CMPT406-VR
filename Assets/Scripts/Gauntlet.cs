using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gauntlet : MonoBehaviour {

    float validSpeed = 10;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

	}

    private void OnTriggerEnter(Collider other)
    {
        //check tag
        if(other.tag == "Enemy")
        {
            //Call enemy's counter function
            //EnemyScript enemyscript = other.gameObject.GetComponent<EnemyScript>();
            if (speedCheck(validSpeed))
            {
                //enemyscript.counter();
            }
        }else if (other.tag == "projectile")
        {
            //call projectile's counter function
            //ProjectileScript projectilescript = other.gameObject.GetComponent<ProjectileScript>();
            if (speedCheck(validSpeed))
            {
                //projectilescript.counter();
            }
        }
    }

    private bool speedCheck(float s)
    {
        float speed = this.GetComponent<Rigidbody>().velocity.magnitude;
        return speed > s;
    }
}
