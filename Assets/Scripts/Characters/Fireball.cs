using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour {

    float LIFETIME = 5f;
    float FLIGHT_SPEED = 18F;
    private float duration = 4f;

    public AudioSource trailingSource;

    public GameObject explosion;

    // Use this for initialization
    void Start() {
        GetComponent<Rigidbody>().velocity = transform.forward.normalized * FLIGHT_SPEED;
        Destroy(gameObject, LIFETIME);
        trailingSource.Play();
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy") || other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        trailingSource.Stop();
        explode();
    }

    void explode() {
        foreach (Collider enemy in Physics.OverlapSphere(transform.position, 6f, LayerMask.GetMask("Enemy")))
            burn(enemy);
        Destroy( Instantiate(explosion, transform.position, transform.rotation), 2.0f );
        Destroy(gameObject);
    }

    void burn(Collider other) {
        if (other.tag != "TutorialEnemy") //don't damage tutorial enemies
            other.SendMessage("startBurning", duration);
    }
}
