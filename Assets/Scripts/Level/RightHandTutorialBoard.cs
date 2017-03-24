using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightHandTutorialBoard : MonoBehaviour {
    

    Renderer ren1;
    Renderer ren2;
    Renderer ren3;

    // Use this for initialization
    void Start () {
        GameObject board1 = GameObject.Find("RightBoard1");
        GameObject board2 = GameObject.Find("RightBoard2");
        GameObject board3 = GameObject.Find("5");

        ren1 = board1.GetComponent<Renderer>();
        ren1.enabled = true;
        ren2 = board2.GetComponent<Renderer>();
        ren2.enabled = false;
        ren3 = board3.GetComponent<Renderer>();
        ren3.enabled = true;
    }
	
	// Update is called once per frame
	void Update () {
		if (OVRInput.GetDown(OVRInput.Button.One)) {
            if (ren1.enabled) {
                ren1.enabled = false;
                ren2.enabled = true;
                ren3.enabled = false;
            }
            else {
                ren1.enabled = true;
                ren2.enabled = false;
                ren3.enabled = true;
            }
        }
	}
}
