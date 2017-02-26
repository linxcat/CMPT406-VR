using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
    float startTime;
	// Use this for initialization
	void Start () {
        startTime = Time.timeSinceLevelLoad;
    }
	
	// Update is called once per frame
	void Update () {
        //delay to match arm throwing motion
        //if (Time.time - startTime > 0.4f) {
            transform.position = Vector3.MoveTowards(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position, 10 * Time.deltaTime);
        //}
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject == GameObject.FindGameObjectWithTag("Player")) {
            Destroy(this.gameObject);
        }
    }
}
