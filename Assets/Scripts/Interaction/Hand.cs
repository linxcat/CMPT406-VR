using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour {

    public bool IS_PRIMARY;

    float SIGIL_DISTANCE = 0.05F;
    float HIT_ARRAY_DISTANCE = 0.6F;
    float HIT_ARRAY_VERT_BIAS = 0.2F;

    bool swordIsOn = true;
    bool locked = false;
    bool locking = false;

    float speedThreshold = 0.025F;
    private float[] pastSpeeds = new float[10];

    GameObject sigilAnchor;
    GameObject hitArray;
    Sword sword;
    Transform teleLineSpawn;
    MagicDraw magicDraw;
    Spells spells;
    GameObject centerEyeAnchor;
    Teleport teleportScript;

    void Awake() {
        sigilAnchor = GameObject.Find("SigilAnchor"); //need before the other hand sets it inactive
    }

    // Use this for initialization
	void Start () {
        hitArray = GameObject.Find("HitArray");
        sword = transform.parent.Find("SwordAnchor/Sword").GetComponent<Sword>();
        teleLineSpawn = transform.Find("teleLineSpawn");
        magicDraw = transform.Find("DrawTouch").gameObject.GetComponent<MagicDraw>();
        spells = GameObject.Find("Player").GetComponent<Spells>();
        centerEyeAnchor = GameObject.Find("CenterEyeAnchor");
        teleportScript = GameObject.Find("Player").GetComponent<Teleport>();
        initialize();
        StartCoroutine("trackSpeed");
    }
	
	// Update is called once per frame
	void Update () {
        if (IS_PRIMARY) {
            if (!swordIsOn && !locking) updateSigilAnchor();
            if (!swordIsOn && locking) magicDraw.gameObject.SetActive(true);
            if (!swordIsOn && !locking && locked) doMagic();
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

    void initialize() {
        swordIsOn = false;
        if (IS_PRIMARY) {
            teleportScript.setTeleLineSpawn(teleLineSpawn);
            switchMode();
        }
        else {
            sword.gameObject.SetActive(false);
            magicDraw.gameObject.SetActive(false);
        }
    }

    // switches the hand, if it's the dominant one, between sword and magic
    public void switchMode() {
        swordIsOn = !swordIsOn;
        sword.gameObject.SetActive(swordIsOn);
        hitArray.SetActive(swordIsOn);
        sigilAnchor.SetActive(!swordIsOn);
        // TODO handle active slashes and sigils on switch
    }

    void updateSigilAnchor()
    {
        Vector3 sigilDirection = transform.position - centerEyeAnchor.transform.position;
        sigilDirection.Normalize();
        Vector3 sigilTransform = sigilDirection * SIGIL_DISTANCE;
        sigilAnchor.transform.position = transform.position + sigilTransform;

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

    IEnumerator trackSpeed() {
        int counter = 0;
        Vector3 lastPoint = transform.position;
        while (true) {
            pastSpeeds[counter++] = (transform.position - lastPoint).magnitude;
            lastPoint = transform.position;
            if (counter == pastSpeeds.Length) counter = 0;
            yield return null;
        }
    }

    private float getSpeed() {
        float totalSpeed = 0F;
        for (int i = 0; i < pastSpeeds.Length; i++) {
            totalSpeed += pastSpeeds[i];
        }
        return totalSpeed / pastSpeeds.Length;
    }

    void OnTriggerEnter(Collider other) {
        if (IS_PRIMARY) return;

        if (getSpeed() > speedThreshold) {
            other.SendMessageUpwards("counter");
        }
    }

    void doMagic() {
        string spell = magicDraw.getSpell();
        magicDraw.clear();
        spells.cast(spell);
    }

    public void setFingerOut(bool value) {
        magicDraw.setDrawing(value);
    }

    public void switchDebug() {
        sword.switchDebug();
    }

    public void setLock(bool value)
    {
        locking = value;
    }

    public void chargeSword(bool charge) {
		if (charge) {
			sword.StartCoroutine ("swordCharge");
		} else {
			sword.stopSound();
			sword.StopCoroutine ("swordCharge");

		}
    }

    public void switchPrimaryHand()
    {
        IS_PRIMARY = !IS_PRIMARY;
        initialize();
    }
}
