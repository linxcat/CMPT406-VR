using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
    GameObject originator;
    GameObject target;
    GameObject explode;
    Vector3 orignalDirection;
    float speed = 0.15f;
    float homingSpeed = 7f;

    public AudioSource trailingSource;
	// Use this for initialization
	void Start () {

        target = GameObject.FindGameObjectWithTag("Player");
        orignalDirection = originator.transform.forward;
        trailingSource.Play();

    }

    public void setOriginator(GameObject origin) {
        originator = origin;
    }
	
	// Update is called once per frame
	void Update () {



        gameObject.transform.position += orignalDirection * speed * Time.timeScale;
        

        
    }

    private void OnCollisionEnter(Collision collision) {

        if (collision.gameObject == target.gameObject) {
            GameObject x = (GameObject) Instantiate(explode);
            x.transform.position = transform.position;
            trailingSource.Stop();
            Destroy(gameObject);
        }
    }
    public void reflect() {
        target = originator;
        orignalDirection *= -1;
    }

    public void absorb() {
        //replace this code with code that increases mana
        Destroy(gameObject);
    }

    public void setTarget(GameObject value) {
        target = value;
    }
}
