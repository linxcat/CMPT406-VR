using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordChargeShot : MonoBehaviour {

    float LIFETIME = 5F;
    float FLIGHT_SPEED = 12F;

	// Use this for initialization
	void Start () {
        GetComponent<Rigidbody>().velocity = transform.forward.normalized * FLIGHT_SPEED;
        Destroy(gameObject, LIFETIME);
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy")) {
            other.gameObject.SendMessage("takeDamage", 50);
            Destroy(gameObject);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Ground")) {
            Destroy(gameObject);
        }
        
        // call damage on any enemy we hit, they will destroy us if necessary
    }
}
