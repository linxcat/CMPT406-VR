using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    Material colourMaterial;
    GameObject weapon;
    Transform pivot;

    Vector3 weaponStartPosition;

    float swingSpeed = 1.75F; //note, the flips in swinging must be > swingSpeed/2
    bool swingDown = true;

    float parryTime = 2F;

	// Use this for initialization
	void Start () {
        foreach (Transform child in transform)
        {
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
                colourMaterial.SetColor("_Color", Color.blue);
                break;
            case Hit.ACCURACY.Good:
                colourMaterial.SetColor("_Color", Color.green);
                break;
            case Hit.ACCURACY.Bad:
                colourMaterial.SetColor("_Color", Color.red);
                break;
        }
        yield return new WaitForSeconds(0.75F);
        colourMaterial.SetColor("_Color", Color.white);
    }

    IEnumerator swingWeapon(float delay)
    {
        yield return new WaitForSeconds(delay);
        weapon.GetComponent<Renderer>().material.SetColor("_Color", Color.white);

        while (true)
        {
            if (swingDown)
            {
                weapon.transform.RotateAround(pivot.position, pivot.right, swingSpeed);
            }
            else
            {
                weapon.transform.RotateAround(pivot.position, pivot.right, -swingSpeed);
            }
            if (swingDown && (Vector3.Angle(weapon.transform.up, pivot.forward) < 1F)) swingDown = false;
            else if (!swingDown && (Vector3.Angle(weapon.transform.up, pivot.up) < 1F)) swingDown = true;

            yield return null;
        }
    }

    public void counter() {
        if (!swingDown) return;

        weapon.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
        weapon.transform.up = pivot.up;
        weapon.transform.localPosition = weaponStartPosition;
        swingDown = true;
        StopCoroutine("swingWeapon");
        StartCoroutine("swingWeapon", parryTime);
    }
}
