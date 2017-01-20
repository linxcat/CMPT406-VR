using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour {

    Hand[] hands = new Hand[2];

	// Use this for initialization
	void Start () {
        hands[0] = GameObject.Find("Hand-Dominant").GetComponent<Hand>();
        hands[1] = GameObject.Find("Hand-Secondary").GetComponent<Hand>();
	}
	
	// Update is called once per frame
	void Update () {
        bool rightModeSwitch = OVRInput.GetDown(OVRInput.Button.One);
        bool locking = OVRInput.Get(OVRInput.Button.SecondaryHandTrigger);
        if (rightModeSwitch) switchHandMode();
        lockHand(locking);
	}

    void switchHandMode()
    {
        foreach (Hand hand in hands)
        {
            hand.switchMode();
        }
    }

    void lockHand(bool locking)
    {
        foreach (Hand hand in hands)
        {
            hand.lockSigils(locking);
        }
    }
}
