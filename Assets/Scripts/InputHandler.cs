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
        bool modeSwitch = OVRInput.GetDown(OVRInput.Button.One);
        bool debugSwitch = OVRInput.GetDown(OVRInput.Button.Three);
        bool locking = OVRInput.Get(OVRInput.Button.SecondaryHandTrigger);
        if (modeSwitch) switchHandMode();
        if (debugSwitch) switchDebug();
        lockHand(locking);
	}

    void switchHandMode()
    {
        foreach (Hand hand in hands)
        {
            hand.switchMode();
        }
    }

    void switchDebug() {
        foreach (Hand hand in hands) {
            hand.switchDebug();
        }
    }

    void lockHand(bool locking)
    {
        foreach (Hand hand in hands)
        {
            hand.setLock(locking);
        }
    }
}
