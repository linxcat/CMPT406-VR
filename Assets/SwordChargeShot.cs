using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordChargeShot : MonoBehaviour {

    Sword swordScript;
    Vector3 shotVelocity;

	// Use this for initialization
	void Start () {
        //swordScript = GameObject.Find("Sword").GetComponent<Sword>();
        //shotVelocity = this.GetComponent<Rigidbody>().velocity;
        //shotVelocity = swordScript.GetChargeShotDirection();
    }
	
	// Update is called once per frame
	void Update () {
        //shotVelocity = Vector3.Scale(shotVelocity, new Vector3(2f, 2f, 2f));
    }
}
