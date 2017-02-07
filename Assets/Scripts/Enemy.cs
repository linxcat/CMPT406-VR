using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    Material colourMaterial;

	// Use this for initialization
	void Start () {
        colourMaterial = GetComponent<Renderer>().material;
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
}
