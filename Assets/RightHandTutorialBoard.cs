using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightHandTutorialBoard : MonoBehaviour {
    

    Renderer ren1;
    Renderer ren2;

    // Use this for initialization
    void Start () {
        GameObject board1 = GameObject.Find("RightBoard1");
        GameObject board2 = GameObject.Find("RightBoard2");

        ren1 = board1.GetComponent<Renderer>();
        ren1.enabled = true;
        ren2 = board2.GetComponent<Renderer>();
        ren2.enabled = false;
    }
	
	// Update is called once per frame
	void Update () {
		if (OVRInput.GetDown(OVRInput.Button.One)) {
            if (ren1.enabled) {
                ren1.enabled = false;
                ren2.enabled = true;
            }
            else {
                ren1.enabled = true;
                ren2.enabled = false;
            }
        }
	}
}
