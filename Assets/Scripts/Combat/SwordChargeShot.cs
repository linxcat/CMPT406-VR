using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordChargeShot : MonoBehaviour {

    float LIFETIME = 5F;
    float FLIGHT_SPEED = 2F;

	// Use this for initialization
	void Start () {
        GetComponent<Rigidbody>().velocity = transform.forward.normalized * FLIGHT_SPEED;
        Destroy(gameObject, LIFETIME);
    }

    void OnTriggerEnter(Collider other) {
        other.gameObject.SendMessage("takeDamage", 50);
        // call damage on any enemy we hit, they will destroy us if necessary
    }
}
