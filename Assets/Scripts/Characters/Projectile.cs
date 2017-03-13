using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
    GameObject originator;
    GameObject target;
    GameObject player = GameObject.FindGameObjectWithTag("Player");
    // Use this for initialization
    void Start () {

        target = GameObject.FindGameObjectWithTag("Player");


    }

    public void setOriginator(GameObject origin) {
        originator = origin;
    }
	
	// Update is called once per frame
	void Update () {

    }

    private void OnCollisionEnter(Collision collision) {

        if (collision.gameObject == target.gameObject) {
            Destroy(gameObject);
        }
    }
    public void reflect() {
        target = originator;
    }

    public void absorb() {
        //replace this code with code that increases mana
        Destroy(gameObject);
    }

    public void setTarget(GameObject value) {
        target = value;
    }
}
