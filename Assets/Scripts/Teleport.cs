using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour {

    public const float FADE_DURATION = 0.2f; // TODO change all constants to constant type
    private bool active = false;
    private bool teleporting = false;

    GameObject player;
    GameObject teleHand;
    GameObject avatar;
    Transform apex;
    Transform groundLocation;
    SphereCollider groundSphereCollider;
    int teleportMask;
    int secondArcMask;

    // Use this for initialization
    void Start () {
        player = GameObject.Find("Player");
        teleHand = GameObject.Find("Hand-Dominant"); // TODO pull from InputManager once hands are arbitrary
        apex = GameObject.Find("apex").transform;
        groundLocation = GameObject.Find("groundMarker").transform;
        groundSphereCollider = groundLocation.GetComponent<SphereCollider>();
        teleportMask = LayerMask.GetMask(new string[3] { "Ground", "EnemyRange", "TeleportCollider" });
        secondArcMask = LayerMask.GetMask(new string[2] { "Ground", "EnemyRange" });
    }
	
	// Update is called once per frame
	void Update () {
		if (active) {
            drawArc();
        }
	}

    void drawArc() {
        RaycastHit hitInfo = new RaycastHit();
        if (!Physics.Raycast(teleHand.transform.position, teleHand.transform.forward, out hitInfo, float.MaxValue, teleportMask)) Debug.Log("HOLE IN TELEPORT BOUNDARIES!!");
        int layerOfHit = hitInfo.collider.gameObject.layer;

        
        if (layerOfHit == LayerMask.NameToLayer("TeleportCollider")) {
            apex.transform.position = hitInfo.point;
            Vector3 secondDirection;
            if (hitInfo.collider.tag == "coneCap") {
                Vector3 incomingVec = hitInfo.point - teleHand.transform.position;
                secondDirection = Vector3.Reflect(incomingVec, hitInfo.normal);
            }
            else {
                Vector3 downwardAngle = teleHand.transform.forward;
                downwardAngle.Normalize();
                secondDirection = Quaternion.AngleAxis(-45, -Vector3.up) * downwardAngle;
            }
            RaycastHit secondHit = new RaycastHit();
            Physics.Raycast(hitInfo.point, secondDirection, out secondHit, float.MaxValue, secondArcMask);
            int layerOfSecondHit = secondHit.collider.gameObject.layer;

            if (layerOfSecondHit == LayerMask.NameToLayer("Ground")) groundLocation.position = hitInfo.point;
            else if (layerOfSecondHit == LayerMask.NameToLayer("EnemyRange")) groundLocation.position = findOffsetPoint(secondHit.collider, secondHit.transform.position);

            drawWithApex();
        }
        else if (layerOfHit == LayerMask.NameToLayer("Ground")) {
            groundLocation.position = hitInfo.point;
            drawNoApex();
        }
        else if (layerOfHit == LayerMask.NameToLayer("EnemyRange")) {
            groundLocation.position = findOffsetPoint(hitInfo.collider, hitInfo.transform.position);
            drawNoApex();
        }
    }

    void drawWithApex() {

    }

    void drawNoApex() {

    }

    public void go() {
        if (teleporting) return;
        StartCoroutine("TeleportPosition");
    }

    IEnumerator TeleportPosition() {
        teleporting = true;
        fadeOut();
        avatar.SetActive(false); // otherwise we see hands in the black while teleporting
        yield return new WaitForSeconds(FADE_DURATION);
        avatar.SetActive(true);
        player.transform.position = groundLocation.position;
        player.transform.forward = groundLocation.forward;
        fadeIn();
        teleporting = false;
    }

    public Vector3 findOffsetPoint(Collider collider, Vector3 hitLocation) {
        Vector3 direction = hitLocation - collider.transform.position;
        Vector3.ProjectOnPlane(direction, Vector3.up); // maybe fragile?
        direction.Normalize();
        direction *= (groundSphereCollider.radius);
        hitLocation += direction;
        RaycastHit hitInfo = new RaycastHit();
        Physics.Raycast(hitLocation, -Vector3.up, out hitInfo, float.MaxValue, LayerMask.GetMask(new string[1] { "Ground" }));
        return hitInfo.point;
    }

    public void setRotation(Vector2 vector) {
        active = true;
    }

    public void disable() {
        active = false;
    }

    private void fadeOut() {

    }

    private void fadeIn() {

    }
}
