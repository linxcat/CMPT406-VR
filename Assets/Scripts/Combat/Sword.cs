using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour {

    public float SWINGTIME = 0.12F; // TODO this

    GameObject swordAnchor;
    GameObject handAnchor;
    HitArray hitArray;

    Rigidbody swordRigidbody;

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

    List<Vector3> swordStepPositions = new List<Vector3>();
    List<Vector3> swordStepForwardDirections = new List<Vector3>();
    LinkedList<GameObject> enemyContacts = new LinkedList<GameObject>();

    bool swordCharged = false;
    Vector3 chargeShotPosition;
    Vector3 chargeShotForwardDirection;
    public GameObject ChargeShot;

    public AudioClip vibeAudioClip;
    OVRHapticsClip vibeClip;

    Vector3 chargeShotRotation;

    // Use this for initialization
    void Start () {
        swordAnchor = transform.parent.gameObject;
        handAnchor = swordAnchor.transform.parent.gameObject;
        swordRigidbody = GetComponent<Rigidbody>();
        hitArray = GameObject.Find("Hit Array").GetComponent<HitArray>();
        vibeClip = new OVRHapticsClip(vibeAudioClip);
    }
	
	// Update is called once per frame
	void Update () {
        if (isSwinging) timeStep++; slashStep();
	}

    void OnTriggerEnter(Collider other) {
        // if (other.gameObject.layer != LayerMask.NameToLayer("EnemyHit")) return; // sword layer only collides with enemy layer
        if (isSwinging) enemyContacts.AddFirst(other.gameObject); InitiateHapticFeedback(vibeClip, 1);
    }

    public void startSlash() {
        InitiateHapticFeedback(vibeClip, 1);
        timeStep = 0;
        for (int i = 0; i < directionDeviations.Length; i++) {
            directionDeviations[i] = 0F;
            alignmentDeviations[i] = 0F;
        }
        lastPoint = swordAnchor.transform.position;
        startPoint = swordAnchor.transform.position;
        isSwinging = true;
        swordStepPositions.Clear();
        swordStepForwardDirections.Clear();
    }

    void slashStep() {
        if (timeStep % DAMPENING == 0) {
            Vector3 stepDirection = swordAnchor.transform.position - lastPoint;
            Vector3 stepRotation = swordAnchor.transform.right;
            stepRotation.Normalize();

            accumulateDeviations(stepDirection, stepRotation);

            lastPoint = swordAnchor.transform.position;
            swordStepPositions.Add(lastPoint);
            swordStepForwardDirections.Add(handAnchor.transform.forward);
        }
        // accumuluate deviation from sterotype direction and attentuate by decreasing function of timesteps
        // accumulate crossproduct of anchor x axis and stereotypical vector and attenutate by decreasing function of timesteps
        // update speed for thrusts
    }

    public void stopSlash() {
        InitiateHapticFeedback(vibeClip, 1);
        isSwinging = false;
        StopCoroutine("swordCharge");
        stopPoint = swordAnchor.transform.position;

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

        Hit.DIRECTION correctedDirection = fixDirection(swingDirection);

        Hit hit = new Hit(bestAlignmentDeviation, correctedDirection);
        foreach (GameObject enemy in enemyContacts) {
            enemy.SendMessageUpwards("swingHit", hit);
        }
        enemyContacts.Clear();

        if (IsCharged()) {
            chargeShotPosition = swordStepPositions[swordStepPositions.Count / 2];
            chargeShotForwardDirection = swordStepForwardDirections[swordStepForwardDirections.Count / 2];
            chargeShotRotation = DetermineShotRotation(correctedDirection);
            Debug.Log(chargeShotRotation);
            GameObject chargeShot = Instantiate(ChargeShot, chargeShotPosition, Quaternion.Euler(chargeShotRotation));
            chargeShot.GetComponent<Rigidbody>().velocity = GetChargeShotDirection() * 10f;
            Destroy(chargeShot, 5f);
            swordCharged = false;
        }
        
        if (debugMode) {
            directionDeviationSaves.AddFirst(bestDirectionDeviation);
            alignmentDeviationSaves.AddFirst(bestAlignmentDeviation);
        }
        //Debug.Log("Swing in " + correctedDirection + " direction with deviation of " + bestDirectionDeviation + " direction and " + bestAlignmentDeviation + " alignment.");
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

    Vector3 DetermineShotRotation(Hit.DIRECTION direction)
    {
        Vector3 result = new Vector3(0f, 0f, 0f);
        switch (direction)
        {
            case Hit.DIRECTION.D:
                result = new Vector3(0f, 0f, 0f);
                break;
            case Hit.DIRECTION.DL:
                result = new Vector3(0f, 0f, -45f);
                break;
            case Hit.DIRECTION.L:
                result = new Vector3(0f, 0f, 90f);
                break;
            case Hit.DIRECTION.UL:
                result = new Vector3(0f, 0f, 45f);
                break;
            case Hit.DIRECTION.U:
                result = new Vector3(0f, 0f, 0f);
                break;
            case Hit.DIRECTION.UR:
                result = new Vector3(0f, 0f, -45f);
                break;
            case Hit.DIRECTION.R:
                result = new Vector3(0f, 0f, 90f);
                break;
            case Hit.DIRECTION.DR:
                result = new Vector3(0f, 0f, 45f);
                break;
        }
        return result;
    }

    IEnumerator swordCharge(float duration) {
        yield return new WaitForSeconds(duration);
        swordCharged = true;
    }

    // IsCharged 
    // Check no see if the sword is charged
    public bool IsCharged() {
        return swordCharged;
    }

    public Vector3 GetChargeShotPosition() {
        return chargeShotPosition;
    }

    public Vector3 GetChargeShotDirection() {
        return chargeShotForwardDirection;
    }

    // SwordShot 
    // Check is charged
    // If sword is charged shoot
        // Shoot would check the start and end point
        // Take the median of the start and end point
        // Take the direction from actualDirection
        // Fire shot 
    public void FireChargedShot() {
        if (!IsCharged()) {
            return;
        }
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

    //Call to initiate haptic feedback on a controller depending on the channel perameter. (Left controller is 0, right is 1)
    public void InitiateHapticFeedback(OVRHapticsClip hapticsClip, int channel)
    {
        OVRHaptics.Channels[channel].Mix(hapticsClip);
    }

    public void InitiateHapticFeedback(AudioClip hapticsAudioClip, int channel)
    {
        OVRHapticsClip hapticsClip = new OVRHapticsClip(hapticsAudioClip);
        OVRHaptics.Channels[channel].Mix(hapticsClip);
    }
}
