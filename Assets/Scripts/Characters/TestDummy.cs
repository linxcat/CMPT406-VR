using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDummy : Enemy {

    Material colourMaterial;
    GameObject weapon;
    Transform pivot;

    Vector3 weaponStartPosition;

    float swingSpeed = 1.75F; //note, the flips in swinging must be > swingSpeed/2
    bool swingDown = true;

    float parryTime = 2F;

    // Haptics
    public AudioClip badHapticAudio;
    public AudioClip goodHapticAudio;
    public AudioClip perfectHapticAudio;
    OVRHapticsClip badHapticClip;
    OVRHapticsClip goodHapticClip;
    OVRHapticsClip perfectHapticClip;

    // Use this for initialization
    void Start () {
        base.Start();

        foreach (Transform child in transform) {
            if (child.name == "weapon") {
                weapon = child.gameObject;
                weaponStartPosition = child.localPosition;
            }
            if (child.name == "pivot") {
                pivot = child.transform;
            }
            if (child.name == "model") colourMaterial = child.GetComponent<Renderer>().material;
        }
        StartCoroutine("swingWeapon", 0F);

        //Haptics
        badHapticClip = new OVRHapticsClip(badHapticAudio);
        goodHapticClip = new OVRHapticsClip(goodHapticAudio);
        perfectHapticClip = new OVRHapticsClip(perfectHapticAudio);
    }
	
	// Update is called once per frame
	void Update () {
        slowFacePlayer();
	}

    public override void swingHit(Hit hit) {
        StopCoroutine("colourFlash");
        StartCoroutine("colourFlash", hit.getAccuracy());
    }

    public override void counter() {
        if (!swingDown) return;

        weapon.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
        weapon.transform.up = pivot.up;
        weapon.transform.localPosition = weaponStartPosition;
        swingDown = true;
        StopCoroutine("swingWeapon");
        StartCoroutine("swingWeapon", parryTime);
    }

    public void takeDamage() {
        return;
    }

    public override void die() {
        return;
    }

    IEnumerator colourFlash(Hit.ACCURACY accuracy) {
        switch (accuracy) {
            case Hit.ACCURACY.Perfect:
                audioSource.PlayOneShot(perfectHitClip, 0.2f);
                InitiateHapticFeedback(perfectHapticClip, 1);
                colourMaterial.SetColor("_Color", Color.blue);
                takeDamage(maxDamage);
                Debug.Log("Enemy hit! Damage: " + maxDamage);
                break;
            case Hit.ACCURACY.Good:
                audioSource.PlayOneShot(goodHitClip, 0.2f);
                InitiateHapticFeedback(goodHapticClip, 1);
                colourMaterial.SetColor("_Color", Color.green);
                Debug.Log("Enemy hit! Damage: " + maxDamage / 2);
                break;
            case Hit.ACCURACY.Bad:
                audioSource.PlayOneShot(badHitClip, 0.2f);
                InitiateHapticFeedback(badHapticClip, 1);
                colourMaterial.SetColor("_Color", Color.red);
                Debug.Log("Enemy hit! Damage: " + maxDamage / 4);
                break;
        }
        yield return new WaitForSeconds(0.75F);
        colourMaterial.SetColor("_Color", Color.white);
    }

    IEnumerator swingWeapon(float delay) {
        yield return new WaitForSeconds(delay);
        weapon.GetComponent<Renderer>().material.SetColor("_Color", Color.white);

        while (true) {
            if (swingDown) {
                weapon.transform.RotateAround(pivot.position, pivot.right, swingSpeed);
            }
            else {
                weapon.transform.RotateAround(pivot.position, pivot.right, -swingSpeed);
            }
            if (swingDown && (Vector3.Angle(weapon.transform.up, pivot.forward) < 1F)) swingDown = false;
            else if (!swingDown && (Vector3.Angle(weapon.transform.up, pivot.up) < 1F)) {
                swingDown = true;
                weapon.transform.localPosition = weaponStartPosition;
            }

            yield return null;
        }
    }

}
