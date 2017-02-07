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
    float[] alignmentDeviations = new float[8];

    LinkedList<GameObject> enemyContacts = new LinkedList<GameObject>();

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
        // if (other.gameObject.layer != LayerMask.NameToLayer("EnemyHit")) return; // sword layer only collides with enemy layer
        if (isSwinging) enemyContacts.AddFirst(other.gameObject);
    }

    public void startSlash() {
        timeStep = 0;
        for (int i = 0; i < directionDeviations.Length; i++) {
            directionDeviations[i] = 0F;
            alignmentDeviations[i] = 0F;
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
            enemy.SendMessage("swingHit", hit);
        }
        enemyContacts.Clear();

        // TODO if debug mode, store best deviations into an array for averaging
        Debug.Log("Swing in " + correctedDirection + " direction with deviation of " + bestDirectionDeviation + " direction and " + bestAlignmentDeviation + " alignment.");
    }

    void accumulateDeviations(Vector3 direction, Vector3 alignment) {
        for (int i = 0; i < directionDeviations.Length; i++) {
            directionDeviations[i] += Mathf.Abs((Vector3.Cross(direction, hitArray.getDirection((Hit.DIRECTION)i))).magnitude);
            alignmentDeviations[i] += Mathf.Abs((Vector3.Cross(alignment, hitArray.getDirection((Hit.DIRECTION)i))).magnitude);
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
}
