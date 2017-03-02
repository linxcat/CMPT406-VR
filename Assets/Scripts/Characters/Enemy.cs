using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    //protected Material colourMaterial;
    protected GameObject player;
    public int hp = 100;
    public int maxDamage = 50;
 
    public AudioSource audioSource;
    public AudioClip badHitClip;
    public AudioClip goodHitClip;
    public AudioClip perfectHitClip;

    // Use this for initialization
    public void Start () {
        audioSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void swingHit(Hit hit) {
        StopCoroutine("colourFlash");
        StartCoroutine("colourFlash", hit.getAccuracy());
    }

    IEnumerator colourFlash(Hit.ACCURACY accuracy) {
        switch (accuracy) {
            case Hit.ACCURACY.Perfect:
                audioSource.PlayOneShot(perfectHitClip, 0.2f);
                //colourMaterial.SetColor("_Color", Color.blue);
                takeDamage(maxDamage);
                Debug.Log("Enemy hit! Damage: " + maxDamage);
                break;
            case Hit.ACCURACY.Good:
                audioSource.PlayOneShot(goodHitClip, 0.2f);
                //colourMaterial.SetColor("_Color", Color.green);
                takeDamage(maxDamage/2);
                Debug.Log("Enemy hit! Damage: " + maxDamage/2);
                break;
            case Hit.ACCURACY.Bad:
                audioSource.PlayOneShot(badHitClip, 0.2f);
                //colourMaterial.SetColor("_Color", Color.red);
                takeDamage(maxDamage/4);
                Debug.Log("Enemy hit! Damage: " + maxDamage/4);
                break;
        }
        yield return new WaitForSeconds(0.75F);
        //colourMaterial.SetColor("_Color", Color.white);
    }

    protected void facePlayer(Vector3 other)
    {
        transform.LookAt(other);
    }

    virtual protected void takeDamage(int damage)
    {
        this.hp = this.hp - damage;
    }

    public bool isAlive()
    {
        if (hp <= 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public int getHp()
    {
        return this.hp;
    }
}
