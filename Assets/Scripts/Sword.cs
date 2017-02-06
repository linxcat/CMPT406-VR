using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour {

    public float SWINGTIME = 0.12F; // TODO this

    GameObject swordAnchor;
    HitArray hitArray;

    bool isSwinging = false;
    int timeStep = 0;
    int DAMPENING = 5;
    Vector3 startPoint;
    Vector3 lastPoint;
    Vector3 stopPoint;
    float[] directionDeviations = new float[8];
    float[] rotationDeviations = new float[8];

    // Use this for initialization
    void Start () {
        swordAnchor = transform.parent.gameObject;
        hitArray = GameObject.Find("Hit Array").GetComponent<HitArray>();
	}
	
	// Update is called once per frame
	void Update () {
        if (isSwinging) timeStep++; slashStep();
	}

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer != LayerMask.NameToLayer("EnemyHit")) return;
    }

    public void startSlash() {
        timeStep = 0;
        for (int i = 0; i < directionDeviations.Length; i++) {
            directionDeviations[i] = 0F;
            rotationDeviations[i] = 0F;
        }
        lastPoint = swordAnchor.transform.position;
        startPoint = swordAnchor.transform.position;
        isSwinging = true;
    }

    void slashStep() {
        if (timeStep % DAMPENING == 0) {
            Vector3 stepDirection = swordAnchor.transform.position - lastPoint;
            Vector3 stepRotation = swordAnchor.transform.right;
            stepRotation.Normalize();

            accumulateDeviations(stepDirection, stepRotation);

            lastPoint = swordAnchor.transform.position;
        }
        // accumuluate deviation from sterotype direction and attentuate by decreasing function of timesteps
        // accumulate crossproduct of anchor x axis and stereotypical vector and attenutate by decreasing function of timesteps
        // update speed for thrusts
    }

    public void stopSlash() {
        isSwinging = false;
        stopPoint = swordAnchor.transform.position;
        float bestDirectionDeviation = float.MaxValue;
        float bestRotationDeviation = float.MaxValue;
        SwingSet.DIRECTIONS swingDirection = SwingSet.DIRECTIONS.Thrust;
        for (int i = 0; i < directionDeviations.Length; i++) {
            if (directionDeviations[i] < bestDirectionDeviation) {
                bestDirectionDeviation = directionDeviations[i];
                bestRotationDeviation = rotationDeviations[i];
                swingDirection = (SwingSet.DIRECTIONS)i;
            }
        }
        SwingSet.DIRECTIONS correctDirection = fixDirection(swingDirection);
        Debug.Log("Swing in " + correctDirection + " direction with deviation of " + bestDirectionDeviation + " direction and " + bestRotationDeviation + " rotation.");
    }

    void accumulateDeviations(Vector3 direction, Vector3 rotation) {
        for (int i = 0; i < directionDeviations.Length; i++) {
            directionDeviations[i] += Mathf.Abs((Vector3.Cross(direction, hitArray.getDirection((SwingSet.DIRECTIONS)i))).magnitude);
            rotationDeviations[i] += Mathf.Abs((Vector3.Cross(rotation, hitArray.getDirection((SwingSet.DIRECTIONS)i))).magnitude);
        }
    }

    SwingSet.DIRECTIONS fixDirection(SwingSet.DIRECTIONS assumedDirection) {
        SwingSet.DIRECTIONS actualDirection = assumedDirection;
        switch (assumedDirection) {
            case SwingSet.DIRECTIONS.D:
                if (startPoint.y < stopPoint.y) actualDirection = SwingSet.DIRECTIONS.U;
                break;
            case SwingSet.DIRECTIONS.DL:
                if (startPoint.y < stopPoint.y) actualDirection = SwingSet.DIRECTIONS.UR;
                break;
            case SwingSet.DIRECTIONS.L:
                if (startPoint.x < stopPoint.x) actualDirection = SwingSet.DIRECTIONS.R;
                break;
            case SwingSet.DIRECTIONS.UL:
                if (startPoint.y > stopPoint.y) actualDirection = SwingSet.DIRECTIONS.DR;
                break;
            case SwingSet.DIRECTIONS.U:
                if (startPoint.y > stopPoint.y) actualDirection = SwingSet.DIRECTIONS.D;
                break;
            case SwingSet.DIRECTIONS.UR:
                if (startPoint.y > stopPoint.y) actualDirection = SwingSet.DIRECTIONS.DL;
                break;
            case SwingSet.DIRECTIONS.R:
                if (startPoint.x > stopPoint.x) actualDirection = SwingSet.DIRECTIONS.L;
                break;
            case SwingSet.DIRECTIONS.DR:
                if (startPoint.y < stopPoint.y) actualDirection = SwingSet.DIRECTIONS.UL;
                break;
        }
        return actualDirection;
    }
}
