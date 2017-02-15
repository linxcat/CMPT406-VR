using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleboundsFollow : MonoBehaviour {

    GameObject centerEyeAnchor;

    // Use this for initialization
    void Start () {
        centerEyeAnchor = GameObject.Find("CenterEyeAnchor");
    }
	
	// Update is called once per frame
	void Update () {
        transform.position = centerEyeAnchor.transform.position;
    }
}
