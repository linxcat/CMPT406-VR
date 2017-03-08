using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour {

    Hand leftHand;
    Hand rightHand;
    Teleport teleport;
    Sword sword;
    float thumbstickDeadzone = 0.1F;

    bool truePrimary = false; // is the primary (left) hand dominant?
    OVRInput.Button modeSwitch, debugSwitch, teleButton, chargeButton;
    OVRInput.Touch fingerTouch;
    OVRInput.Axis2D teleThumbstick;


    // Use this for initialization
    void Start () {
        leftHand = GameObject.Find("LeftHand").GetComponent<Hand>();
        rightHand = GameObject.Find("RightHand").GetComponent<Hand>();
        teleport = GameObject.Find("Player").GetComponent<Teleport>();
        rightButtons(); //initialize buttons
    }

  // Update is called once per frame
  void Update () {
        if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger) &&
            OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger) &&
            OVRInput.GetDown(debugSwitch)) {
            switchHandedness();
            return;
        }

        if (OVRInput.GetDown(modeSwitch)) switchHandMode();
        if (OVRInput.GetDown(debugSwitch)) switchDebug();

        if (OVRInput.GetDown(chargeButton)) swordCharge(true);
    if (OVRInput.GetUp(chargeButton)) swordCharge(false);

        Vector2 thumbstickAxis = OVRInput.Get(teleThumbstick);
        bool teleGo = OVRInput.GetDown(teleButton);
        if (thumbstickPassedDeadzone(thumbstickAxis)) {
            teleport.setRotation(thumbstickAxis);
            if (teleGo) teleport.go();
        }
        else
            teleport.disable();

        leftHand.setLock(OVRInput.Get(OVRInput.Button.PrimaryHandTrigger));
        rightHand.setLock(OVRInput.Get(OVRInput.Button.SecondaryHandTrigger));
        setFingerOut(!OVRInput.Get(fingerTouch));
    }

    void switchHandedness() {
        if (truePrimary) {
            truePrimary = false; // switch to right hand
            rightButtons();
            leftHand.switchPrimaryHand();
            rightHand.switchPrimaryHand();
        }
        else {
            truePrimary = true; // switch to left hand
            leftButtons();
            rightHand.switchPrimaryHand();
            leftHand.switchPrimaryHand();
        }
    }
    void rightButtons() {
        modeSwitch = OVRInput.Button.One;
        debugSwitch = OVRInput.Button.PrimaryThumbstick;
        teleButton = OVRInput.Button.Four;
        chargeButton = OVRInput.Button.SecondaryIndexTrigger;
        fingerTouch = OVRInput.Touch.SecondaryIndexTrigger;
        teleThumbstick = OVRInput.Axis2D.SecondaryThumbstick;
    }

    void leftButtons() {
        modeSwitch = OVRInput.Button.Three;
        debugSwitch = OVRInput.Button.SecondaryThumbstick;
        teleButton = OVRInput.Button.Two;
        chargeButton = OVRInput.Button.PrimaryIndexTrigger;
        fingerTouch = OVRInput.Touch.PrimaryIndexTrigger;
        teleThumbstick = OVRInput.Axis2D.PrimaryThumbstick;
    }

    void setFingerOut(bool value) {
        if (truePrimary) leftHand.setFingerOut(value);
        else rightHand.setFingerOut(value);
    }

    void switchHandMode()
    {
        if (truePrimary) leftHand.switchMode();
        else rightHand.switchMode();
    }

    void switchDebug() {
        if (truePrimary) leftHand.switchDebug();
        else rightHand.switchDebug();
    }

    void swordCharge(bool charge) {
    if (truePrimary) {
      leftHand.chargeSword (charge);
    }
    else {
      rightHand.chargeSword (charge);
    }
    }

    bool thumbstickPassedDeadzone(Vector2 thumbstickAxis) {
        return (Mathf.Abs(thumbstickAxis.x) > thumbstickDeadzone || Mathf.Abs(thumbstickAxis.y) > thumbstickDeadzone);
    }
}
