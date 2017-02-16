using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour {

    public bool IS_PRIMARY; // script is present on both hands, but we need left-hand support

    public float SIGIL_DISTANCE = 0.05F;
    public float HIT_ARRAY_DISTANCE = 0.6F;
    public float HIT_ARRAY_VERT_BIAS = 0.2F;

    bool swordIsOn = false;
    bool locked = false;
    bool locking = false;

    public float gauntletSpeedThreshold = 0.025F;
    private float[] pastSpeeds = new float[10];

    GameObject sigilAnchor;
    GameObject hitArray;
    Sword sword;
    GameObject centerEyeAnchor;
    GameObject handAnchor;

    // Use this for initialization
	void Start () {
        sigilAnchor = GameObject.Find("Sigil Anchor");
        hitArray = GameObject.Find("Hit Array");
        sword = GameObject.Find("Sword").GetComponent<Sword>();
        centerEyeAnchor = GameObject.Find("CenterEyeAnchor");
        handAnchor = GameObject.Find("Hand-Dominant"); // TODO find by tag and get primary

        switchMode(); // both must start enabled, this sets the sword to start as on.
        if (!IS_PRIMARY) StartCoroutine("trackGauntletSpeed");
    }
	
	// Update is called once per frame
	void Update () {
        if (IS_PRIMARY) {
            if (!swordIsOn && !locking) updateSigilAnchor();
            if (swordIsOn && !locking) updateHitArray();
            if (swordIsOn && locking && !locked) sword.startSlash();
            if (swordIsOn && !locking && locked) sword.stopSlash();
        }
       else {
            // gauntlet
        }

        if ((locking && !locked) || (!locking && locked)) { // finalize hand lock
            locked = !locked;
        }
	}

    void updateSigilAnchor()
    {
        Vector3 sigilDirection = transform.position - centerEyeAnchor.transform.position;
        sigilDirection.Normalize();
        Vector3 sigilTransform = sigilDirection * SIGIL_DISTANCE;
        sigilAnchor.transform.position = handAnchor.transform.position + sigilTransform;

        sigilAnchor.transform.forward = sigilDirection;
    }

    void updateHitArray() {
        Vector3 headPosition = centerEyeAnchor.transform.position;

        Vector3 headFacing = centerEyeAnchor.transform.forward;
        headFacing.y = 0;
        headFacing.Normalize();

        Vector3 anchorTransform = headPosition + (headFacing * HIT_ARRAY_DISTANCE);
        anchorTransform.y -= HIT_ARRAY_VERT_BIAS;

        hitArray.transform.position = anchorTransform;
        hitArray.transform.forward = headFacing;
    }

    IEnumerator trackGauntletSpeed() {
        int counter = 0;
        Vector3 lastPoint = transform.position;
        while (true) {
            pastSpeeds[counter++] = (transform.position - lastPoint).magnitude;
            lastPoint = transform.position;
            if (counter == pastSpeeds.Length) counter = 0;
            yield return null;
        }
    }

    private float getGauntletSpeed() {
        float totalSpeed = 0F;
        for (int i = 0; i < pastSpeeds.Length; i++) {
            totalSpeed += pastSpeeds[i];
        }
        return totalSpeed / pastSpeeds.Length;
    }

    void OnTriggerEnter(Collider other) {
        if (IS_PRIMARY) return;

        if (getGauntletSpeed() > gauntletSpeedThreshold) {
            other.SendMessageUpwards("counter");
        }
    }

    // switches the hand, if it's the dominant one, between sword and magic
    public void switchMode()
    {
        if (!IS_PRIMARY) return; // only the primary hand can switch, but it's called on both
        swordIsOn = !swordIsOn;
        sword.gameObject.SetActive(swordIsOn);
        hitArray.SetActive(swordIsOn);
        sigilAnchor.SetActive(!swordIsOn);
        // TODO handle active slashes and sigils on switch
    }

    public void switchDebug() {
        if (IS_PRIMARY) sword.switchDebug();
    }

    public void setLock(bool value)
    {
        locking = value;
    }

    public void switchPrimaryHand()
    {
        IS_PRIMARY = !IS_PRIMARY;
    }
}
