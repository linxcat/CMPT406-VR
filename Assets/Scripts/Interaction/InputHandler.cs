﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour {

    Hand[] hands = new Hand[2];
    Teleport teleport;
    Sword sword;
    float thumbstickDeadzone = 0.1F;

    // Use this for initialization
    void Start () {
        hands[0] = GameObject.Find("Hand-Dominant").GetComponent<Hand>();
        hands[1] = GameObject.Find("Hand-Secondary").GetComponent<Hand>();
        teleport = GameObject.Find("Player").GetComponent<Teleport>();
        sword = GameObject.Find("Sword").GetComponent<Sword>();
    }
	
	// Update is called once per frame
	void Update () {
        bool modeSwitch = OVRInput.GetDown(OVRInput.Button.One);
        bool debugSwitch = OVRInput.GetDown(OVRInput.Button.Three);
        bool locking = OVRInput.Get(OVRInput.Button.SecondaryHandTrigger);
        Vector2 thumbstickAxis = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick); // TODO hand switching
        bool thumbstickClick = OVRInput.GetDown(OVRInput.Button.Four);

        if (thumbstickPassedDeadzone(thumbstickAxis)) {
            teleport.setRotation(thumbstickAxis);
            if (thumbstickClick) teleport.go();
        }
        else
            teleport.disable();

        if (modeSwitch) switchHandMode();
        if (debugSwitch) switchDebug();
        lockHand(locking);

        if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger)) {
            sword.StartCoroutine("swordCharge");
        }
        if (OVRInput.GetUp(OVRInput.Button.SecondaryIndexTrigger)) {
            sword.StopCoroutine("swordCharge");
        }

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

    bool thumbstickPassedDeadzone(Vector2 thumbstickAxis) {
        return (Mathf.Abs(thumbstickAxis.x) > thumbstickDeadzone || Mathf.Abs(thumbstickAxis.y) > thumbstickDeadzone);
    }
}
