using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour {

    public bool IS_PRIMARY; // script is present on both hands, but we need left-hand support

    public float SIGIL_DISTANCE = 1.5F;

    bool swordIsOn = false;
    bool locked = false;

    GameObject sigilAnchor;
    GameObject swordAnchor;
    GameObject centerEyeAnchor;
    GameObject handAnchor;

    // Use this for initialization
	void Start () {
        sigilAnchor = GameObject.Find("Sigil Anchor");
        swordAnchor = GameObject.Find("Sword Anchor");
        centerEyeAnchor = GameObject.Find("CenterEyeAnchor");
        handAnchor = GameObject.Find("Hand-Dominant"); // TODO find by tag and get primary

        switchMode();
    }
	
	// Update is called once per frame
	void Update () {
        updateSigilAnchor();
	}

    void updateSigilAnchor()
    {
        if (!IS_PRIMARY || locked) return;

        Vector3 sigilDirection = transform.position - centerEyeAnchor.transform.position;
        sigilDirection.Normalize();
        Vector3 sigilTransform = sigilDirection * SIGIL_DISTANCE;
        sigilAnchor.transform.position = handAnchor.transform.position + sigilTransform;

        sigilAnchor.transform.forward = sigilDirection;
    }

    // switches the hand, if it's the dominant one, between sword and magic
    public void switchMode()
    {
        if (!IS_PRIMARY) return; // only the primary hand can switch
        swordIsOn = !swordIsOn;
        swordAnchor.SetActive(swordIsOn);
        sigilAnchor.SetActive(!swordIsOn);
    }

    public void lockSigils(bool locking)
    {
        if (!IS_PRIMARY) return;

        locked = locking;

    }

    public void switchPrimaryHand()
    {
        IS_PRIMARY = !IS_PRIMARY;
    }
}
