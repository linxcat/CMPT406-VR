using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
    GameObject originator;
    GameObject target;

    CharacterStats playerStats;
    int PROJECTILE_MANA = 15;
    int PROJECTILE_DAMAGE = 40;

    bool reflected = false;

    float speed = 0.15f;
    float homingSpeed = 7f;

    public AudioSource trailingSource;
	// Use this for initialization
	void Start () {
        target = GameObject.FindGameObjectWithTag("MainCamera");
        playerStats = FindObjectOfType<CharacterStats>();
        transform.forward = originator.transform.forward + new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));
        trailingSource.Play();
    }

    public void setOriginator(GameObject origin) {
        originator = origin;
    }
	
	// Update is called once per frame
	void Update () {
        if (target != null && Vector3.Distance(transform.position, target.transform.position) < 4f) {
            Quaternion a = transform.rotation;
            Quaternion b = Quaternion.LookRotation(target.transform.position - transform.position, Vector3.up);
            transform.rotation = Quaternion.Lerp(a, b, 0.1f);
        }
            gameObject.transform.position += transform.forward * speed * Time.timeScale;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "PlayerHitBox") {
            other.SendMessage("getHit", PROJECTILE_DAMAGE);
            trailingSource.Stop();
            Destroy(gameObject);
        }
        else if (reflected && other.gameObject.layer == LayerMask.NameToLayer("Enemy")) {
            other.SendMessage("takeDamage", PROJECTILE_DAMAGE);
            trailingSource.Stop();
            Destroy(gameObject);
        }
    }
    public void reflect() {
        target = originator;
        reflected = true;
        transform.forward *= -1;
    }

    public void absorb() {
        playerStats.addMana(PROJECTILE_MANA);
        Destroy(gameObject);
    }

    public void setTarget(GameObject value) {
        target = value;
    }
}
