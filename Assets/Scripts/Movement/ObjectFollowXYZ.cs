using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFollowXYZ : MonoBehaviour {

    GameObject centerEyeAnchor;

    // Use this for initialization
    void Start () {
        centerEyeAnchor = GameObject.Find("CenterEyeAnchor");
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 a = centerEyeAnchor.transform.position;
        Vector3 newPosition = new Vector3(a.x, a.y, a.z);
        transform.position = newPosition;
    }
}
