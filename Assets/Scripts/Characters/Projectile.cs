using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
    GameObject originator;
    GameObject target;
	// Use this for initialization
	void Start () {
      
        
        
    }

    public void setOriginator(GameObject origin) {
        originator = origin;
    }
	
	// Update is called once per frame
	void Update () {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, 10 * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Hand") {
            if (!collision.gameObject.GetComponent<Hand>().IS_PRIMARY) {
                
                collision.gameObject.GetComponent<Hand>().counterProjectile();
                reflect();
            }
        }
        if (collision.gameObject == target.gameObject) {
            Destroy(gameObject);
        }
    }

    public void reflect() {
        target = originator;
    }

    public void absorb() {
        //replace this code with code that increases mana
        Destroy(this.gameObject);
    }

    public void setTarget(GameObject value) {
        target = value;
    }
}
