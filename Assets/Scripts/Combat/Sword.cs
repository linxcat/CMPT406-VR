using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour {

    float SWINGTIME = 0.4F;
    float MIN_SWING_DISTANCE = 1.2F;
    public bool swingTimeExceeded;

    HitArray hitArray;
    Transform centerEyeAnchor;

    bool isSwinging = false;
    int timeStep = 0;
    int DAMPENING = 5;

    bool debugMode = false;
    LinkedList<double> directionDeviationSaves = new LinkedList<double>();
    LinkedList<double> alignmentDeviationSaves = new LinkedList<double>();

    Vector3 startPoint;
    Vector3 lastPoint;
    Vector3 stopPoint;

    float[] directionDeviations = new float[8];
    float[] alignmentDeviations = new float[8];

    LinkedList<GameObject> enemyContacts = new LinkedList<GameObject>();

    bool swordCharged = false;
    float CHARGE_DURATION = 2F;
    public GameObject ChargeShot;

    bool fireballCharged = false;
    public GameObject Fireball;

    public GameObject slashEffect;

    public AudioClip vibeAudioClip;
    OVRHapticsClip vibeClip;
    public AudioClip swordChargeHapticAudio;
    OVRHapticsClip swordChargeHapticClip;

    public AudioSource audioSource;
    public AudioClip swordDrawClip;
    public AudioClip swordChargingClip;
    public AudioClip swordChargedClip;
    public AudioClip swordUndrawClip;

    private CharacterStats characterStats;
    private int chargeShotCost = 100;
    MeshRenderer renderer;
    public Material activeMaterial;
    public Material inactiveMaterial;
    public Material chargedMaterial;
    public Material fireMaterial;

    // Use this for initialization
    void Start () {
        hitArray = GameObject.Find("HitArray").GetComponent<HitArray>();
        centerEyeAnchor = GameObject.Find("CenterEyeAnchor").transform;
        vibeClip = new OVRHapticsClip(vibeAudioClip);
        swordChargeHapticClip = new OVRHapticsClip(swordChargeHapticAudio);
        audioSource = GetComponent<AudioSource>();
        characterStats = FindObjectOfType<CharacterStats> ();
        renderer = GetComponent<MeshRenderer>();
        renderer.material = inactiveMaterial;
    }

    // Update is called once per frame
    void Update () {
        if (isSwinging) {
            timeStep++;
            slashStep();
        }
    }

    void OnTriggerEnter(Collider other) {
        if (isSwinging) {
            enemyContacts.AddFirst(other.gameObject);
            InitiateHapticFeedback(vibeClip, 1);
        }
    }

    public void startSlash() {
        stopSound();
        renderer.material = activeMaterial;
        audioSource.PlayOneShot(swordDrawClip);
        InitiateHapticFeedback(vibeClip, 1);
        timeStep = 0;
        for (int i = 0; i < directionDeviations.Length; i++) {
            directionDeviations[i] = 0F;
            alignmentDeviations[i] = 0F;
        }
        lastPoint = transform.position;
        startPoint = transform.position;
        isSwinging = true;
        StartCoroutine("swingTimeMax");
    }

    void slashStep() {
        if (timeStep % DAMPENING == 0) {
            Vector3 stepDirection = transform.position - lastPoint;
            Vector3 stepRotation = transform.right;
            stepRotation.Normalize();

            accumulateDeviations(stepDirection, stepRotation);

            lastPoint = transform.position;
        }
        // accumuluate deviation from sterotype direction and attentuate by decreasing function of timesteps
        // accumulate crossproduct of anchor x axis and stereotypical vector and attenutate by decreasing function of timesteps
        // TODO update speed for thrusts
    }

    public void stopSlash() {
        StopCoroutine("swordCharge");
        StopCoroutine("swingTimeMax");
        renderer.material = inactiveMaterial;
        audioSource.PlayOneShot(swordUndrawClip);
        InitiateHapticFeedback(vibeClip, 1);
        isSwinging = false;
        stopPoint = transform.position;

        float bestDirectionDeviation = float.MaxValue;
        float bestAlignmentDeviation = float.MaxValue;
        Hit.DIRECTION swingDirection = Hit.DIRECTION.Thrust;

        for (int i = 0; i < directionDeviations.Length; i++) {
            if (directionDeviations[i] < bestDirectionDeviation) {
                bestDirectionDeviation = directionDeviations[i];
                bestAlignmentDeviation = alignmentDeviations[i];
                swingDirection = (Hit.DIRECTION)i;
            }
        }

        Vector3 spawnOffset = (stopPoint - startPoint);
        Quaternion shotDirection = Quaternion.LookRotation(transform.up, transform.right);

        if (swordCharged) {
            FireChargedShot(shotDirection);
            swordCharged = false;
            renderer.material = inactiveMaterial;
        }
        else if (fireballCharged) {
            FireFireball(shotDirection);
            fireballCharged = false;
            renderer.material = inactiveMaterial;
        }

        Hit.DIRECTION correctedDirection = fixDirection(swingDirection);
        Hit hit = new Hit(bestAlignmentDeviation, correctedDirection);

        if (spawnOffset.magnitude > MIN_SWING_DISTANCE) {
            foreach (GameObject enemy in enemyContacts) {
                CreateSlashEffect(enemy, startPoint, spawnOffset); //incorrect so far
                enemy.SendMessageUpwards("swingHit", hit);
            }
        }

        enemyContacts.Clear();
      
        if (debugMode) {
            directionDeviationSaves.AddFirst(bestDirectionDeviation);
            alignmentDeviationSaves.AddFirst(bestAlignmentDeviation);
        }
    }

    void accumulateDeviations(Vector3 direction, Vector3 alignment) {
        for (int i = 0; i < directionDeviations.Length; i++) {
            directionDeviations[i] += Mathf.Abs((Vector3.Cross(direction, hitArray.getDirection((Hit.DIRECTION)i))).magnitude);

            Vector3 planarAlignment = Vector3.ProjectOnPlane(alignment, hitArray.getNormal((Hit.DIRECTION)i));
            alignmentDeviations[i] += Mathf.Abs((Vector3.Cross(planarAlignment, hitArray.getDirection((Hit.DIRECTION)i))).magnitude);
        }
    }

    Hit.DIRECTION fixDirection(Hit.DIRECTION assumedDirection) {
        Hit.DIRECTION actualDirection = assumedDirection;
        switch (assumedDirection) {
            case Hit.DIRECTION.D:
                if (startPoint.y < stopPoint.y) actualDirection = Hit.DIRECTION.U;
                break;
            case Hit.DIRECTION.DL:
                if (startPoint.y < stopPoint.y) actualDirection = Hit.DIRECTION.UR;
                break;
            case Hit.DIRECTION.L:
                if (startPoint.x < stopPoint.x) actualDirection = Hit.DIRECTION.R;
                break;
            case Hit.DIRECTION.UL:
                if (startPoint.y > stopPoint.y) actualDirection = Hit.DIRECTION.DR;
                break;
            case Hit.DIRECTION.U:
                if (startPoint.y > stopPoint.y) actualDirection = Hit.DIRECTION.D;
                break;
            case Hit.DIRECTION.UR:
                if (startPoint.y > stopPoint.y) actualDirection = Hit.DIRECTION.DL;
                break;
            case Hit.DIRECTION.R:
                if (startPoint.x > stopPoint.x) actualDirection = Hit.DIRECTION.L;
                break;
            case Hit.DIRECTION.DR:
                if (startPoint.y < stopPoint.y) actualDirection = Hit.DIRECTION.UL;
                break;
        }
        return actualDirection;
    }

    IEnumerator swordCharge() {
        if (!swordCharged && !fireballCharged && chargeShotCost < characterStats.getMana())
        {
            audioSource.PlayOneShot(swordChargingClip);
            InitiateHapticFeedback(swordChargeHapticClip, 1);
            yield return new WaitForSeconds(CHARGE_DURATION);
            stopSound();
            audioSource.PlayOneShot(swordChargedClip);
            InitiateHapticFeedback(vibeClip, 1);
            swordCharged = true;
            renderer.material = chargedMaterial;
        }
    }

    IEnumerator swingTimeMax() {
        swingTimeExceeded = false;
        yield return new WaitForSeconds(SWINGTIME);
        swingTimeExceeded = true;
        stopSlash(); //force stop
    }

    public void stopSound() {
        audioSource.Stop();
        OVRHaptics.LeftChannel.Clear();
        OVRHaptics.RightChannel.Clear();
    }

    void FireChargedShot(Quaternion direction) {
        characterStats.removeMana (chargeShotCost);
        Instantiate(ChargeShot, this.transform.position, direction);
    }

    void FireFireball(Quaternion direction) {
        Instantiate(Fireball, this.transform.position, direction);
    }

    public bool storeFireball() {
        if(fireballCharged)
            return false;

        fireballCharged = true;
        renderer.material = fireMaterial;
        return true;
    }

    void CreateSlashEffect(GameObject enemy, Vector3 startlocation, Vector3 facing) {
        GameObject slash = Instantiate(slashEffect, enemy.GetComponent<Collider>().ClosestPointOnBounds(centerEyeAnchor.position), Quaternion.identity);
        Vector3 flatSlash = Vector3.ProjectOnPlane(facing, centerEyeAnchor.forward);
        slash.transform.forward = flatSlash;
        slash.GetComponent<ParticleSystem>().Play();
        GameObject.Destroy (slash, 0.5f);
	  }

    //Call to initiate haptic feedback on a controller depending on the channel perameter. (Left controller is 0, right is 1)
    public void InitiateHapticFeedback(OVRHapticsClip hapticsClip, int channel) {
        OVRHaptics.Channels[channel].Mix(hapticsClip);
    }

    public void InitiateHapticFeedback(AudioClip hapticsAudioClip, int channel) {
        OVRHapticsClip hapticsClip = new OVRHapticsClip(hapticsAudioClip);
        OVRHaptics.Channels[channel].Mix(hapticsClip);
    }

    public void switchDebug() {
        if (debugMode) {
            double averageDirectionDeviation = 0.0;
            double averageAlignmentDeviation = 0.0;
            double stdDevDirection = 0.0;
            double stdDevAlignment = 0.0;

            double M = 0.0;
            double S = 0.0;
            int k = 1;
            foreach (double directionDeviation in directionDeviationSaves) {
                averageDirectionDeviation += directionDeviation; //average

                double tmpM = M; //std deviation
                M += (directionDeviation - tmpM) / k;
                S += (directionDeviation - tmpM) * (directionDeviation - M);
                k++;
            }
            averageDirectionDeviation = averageDirectionDeviation / directionDeviationSaves.Count;
            stdDevDirection = System.Math.Sqrt(S / (k - 2));

            M = 0.0;
            S = 0.0;
            k = 1;
            foreach (double alignmentDeviation in alignmentDeviationSaves) {
                averageAlignmentDeviation += alignmentDeviation; //average

                double tmpM = M; //std deviation
                M += (alignmentDeviation - tmpM) / k;
                S += (alignmentDeviation - tmpM) * (alignmentDeviation - M);
                k++;
            }
            averageAlignmentDeviation = averageAlignmentDeviation / alignmentDeviationSaves.Count;
            stdDevAlignment = System.Math.Sqrt(S / (k - 2));

            Debug.Log("Average Direction Deviation = " + averageDirectionDeviation +
                "\nStandard Deviation = " + stdDevDirection +
                "\n------------------------" +
                "\nAverage Alignment Deviation: " + averageAlignmentDeviation +
                "\nStandard Deviation = " + stdDevAlignment + "\n");

            directionDeviationSaves.Clear();
            alignmentDeviationSaves.Clear();

            GetComponent<Renderer>().material.SetColor("_Color", Color.white);
        }
        else {
            GetComponent<Renderer>().material.SetColor("_Color", Color.yellow);
        }
        debugMode = !debugMode;
    }
}
