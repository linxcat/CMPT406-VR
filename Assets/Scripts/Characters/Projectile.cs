using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
    GameObject originator;
    GameObject target;
	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, 10 * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject == target.gameObject) {
            Destroy(gameObject);
        }
    }

    public void reflect() {
        target = originator;
    }

    public void setTarget(GameObject value) {
        target = value;
    }
}
