using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour {

    public bool IS_PRIMARY;

    float SIGIL_DISTANCE = 0.05F;
    float HIT_ARRAY_DISTANCE = 0.6F;
    float HIT_ARRAY_VERT_BIAS = 0.2F;

    //this is the projectile that is affected by the slow ui, which there can be only one
    static GameObject currentProjectile = null;
    GameObject player;
    //difference between this and the current time is the duration the time is still slowed


    bool swordIsOn = true;
    bool locked = false;
    bool locking = false;

    float speedThreshold = 0.025F;
    private float[] pastSpeeds = new float[23];

    static GameObject counterUI;
    GameObject sigilAnchor;
    GameObject hitArray;
    //GameObject GUIAnchor;
    Transform wristAnchor;
    Sword sword;
    Transform teleLineSpawn;
    MagicDraw magicDraw;
    Spells spells;
    GameObject centerEyeAnchor;
    Teleport teleportScript;
    LevelManager levelManager;

    public AudioClip hapticAudio;
    OVRHapticsClip hapticClip;

    public AudioSource audioSource;
    public AudioClip counterSound;

    void Awake() {
        sigilAnchor = GameObject.Find("SigilAnchor"); //need before the other hand sets it inactive
    }

    // Use this for initialization
    void Start () {
        player = GameObject.FindGameObjectWithTag("MainCamera");
        hitArray = GameObject.Find("HitArray");
        //GUIAnchor = GameObject.Find("GUIAnchor");
        wristAnchor = transform.FindChild("WristAnchor");
        sword = transform.parent.Find("SwordAnchor/Sword").GetComponent<Sword>();
        teleLineSpawn = transform.Find("teleLineSpawn");
        magicDraw = transform.Find("DrawTouch").gameObject.GetComponent<MagicDraw>();
        spells = GameObject.Find("Player").GetComponent<Spells>();
        centerEyeAnchor = GameObject.Find("CenterEyeAnchor");
        teleportScript = GameObject.Find("Player").GetComponent<Teleport>();
        levelManager = FindObjectOfType<LevelManager>();

        hapticClip = new OVRHapticsClip(hapticAudio);
        audioSource = GetComponent<AudioSource>();

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
            if (swordIsOn && !locking && locked && !sword.swingTimeExceeded) sword.stopSlash();
        }
       else {
            // gauntlet
 


            placeGUI();
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
        if (!swordIsOn) magicDraw.clear();
        // TODO handle active slashes and sigils on switch
    }

    void updateSigilAnchor() {
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

    void placeGUI() {
        //GUIAnchor.transform.position = wristAnchor.position;
        //GUIAnchor.transform.forward = GUIAnchor.transform.position - centerEyeAnchor.transform.position;
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
            InitiateHapticFeedback(hapticClip, 0);
            audioSource.PlayOneShot(counterSound);
            if (other.gameObject.tag == "Projectile"){
                currentProjectile = other.gameObject;
                counterProjectile();

                other.gameObject.GetComponent<Projectile>().reflect();
            }
            else {
                other.SendMessageUpwards("counter");
            }
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

    public void setLock(bool value) {
        locking = value;
    }

    public void chargeSword(bool charge) {
        //TODO Check mana
        if (charge) {
            sword.StartCoroutine ("swordCharge");
        }
        else {
            sword.stopSound();
            sword.StopCoroutine ("swordCharge");
        }
    }

    public bool storeFireball() {
        if (IS_PRIMARY)
            return sword.storeFireball();
        else
            return false;
    }

    public void switchPrimaryHand() {
        IS_PRIMARY = !IS_PRIMARY;
        initialize();
    }

    public void counterProjectile() {
       
        Time.timeScale = 0.111111f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        
        GameObject counterUI = Instantiate<GameObject>(Resources.Load<GameObject>("CounterProjectile"));
        counterUI.transform.position = player.transform.position + centerEyeAnchor.transform.forward * 0.1f - new Vector3(0,0.4f,0);
        counterUI.transform.Rotate(new Vector3(0, 1, 0), 90);
        
    }


    public static void absorb() {
        if(currentProjectile != null) {
            currentProjectile.SendMessage("absorb");
        }
    }

    //Call to initiate haptic feedback on a controller depending on the channel perameter. (Left controller is 0, right is 1)
    public void InitiateHapticFeedback(OVRHapticsClip hapticsClip, int channel) {
        OVRHaptics.Channels[channel].Mix(hapticsClip);
    }
}
