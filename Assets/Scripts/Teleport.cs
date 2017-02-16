using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour {

    public const float FADE_DURATION = 0.2f; // TODO change all constants to constant type
    public float lineSegmentSize = 0.15f;

    private bool active = false;
    private bool teleporting = false;
    private Transform[] linePoints = new Transform[3];
    GameObject player;
    GameObject avatar;
    GameObject fader;
    LineRenderer teleportArc;
    Transform teleLineSpawn;
    Transform apex;
    Transform groundLocation;
    SphereCollider groundSphereCollider;
    int teleportMask;
    int secondArcMask;

    // Use this for initialization
    void Start () {
        player = GameObject.Find("Player");
        avatar = GameObject.Find("LocalAvatar");
        fader = GameObject.Find("Fader");
        fader.SetActive(false);
        teleportArc = GetComponent<LineRenderer>();
        teleLineSpawn = GameObject.Find("teleLineSpawn").transform;
        apex = GameObject.Find("apex").transform;
        groundLocation = GameObject.Find("groundMarker").transform;
        groundSphereCollider = groundLocation.GetComponent<SphereCollider>();
        teleportMask = LayerMask.GetMask(new string[3] { "Ground", "EnemyRange", "TeleportCollider" });
        secondArcMask = LayerMask.GetMask(new string[2] { "Ground", "EnemyRange" });

        linePoints[0] = teleLineSpawn;
        linePoints[1] = apex;
        linePoints[2] = groundLocation;
    }
	
	// Update is called once per frame
	void Update () {
		if (active && !teleporting) {
            teleportArc.enabled = true;
            groundLocation.gameObject.SetActive(true);
            setPoints();
            setSmoothedPoints();
        }
        else {
            teleportArc.enabled = false;
            groundLocation.gameObject.SetActive(false);
        }
	}

    void setPoints() {
        RaycastHit hitInfo;
        RaycastHit secondHit;

        Physics.Raycast(teleLineSpawn.position, teleLineSpawn.forward, out hitInfo, float.MaxValue, teleportMask);
        if (hitInfo.collider == null) {
            Debug.LogError("HOLE IN TELEPORT BOUNDARIES!!");
            return;
        }
        int layerOfHit = hitInfo.collider.gameObject.layer;
        

        if (layerOfHit == LayerMask.NameToLayer("TeleportCollider")) { 
            Vector3 secondDirection;
            if (hitInfo.collider.tag == "coneCap") {
                Vector3 incomingVec = hitInfo.point - teleLineSpawn.position;
                secondDirection = Vector3.Reflect(incomingVec, hitInfo.normal);
            }
            else {
                Vector3 downwardAngle = teleLineSpawn.forward;
                Vector3.ProjectOnPlane(downwardAngle, Vector3.up);
                Vector3 flattenedHandAxis = Vector3.ProjectOnPlane(teleLineSpawn.right, Vector3.up);
                secondDirection = Quaternion.AngleAxis(45, flattenedHandAxis) * downwardAngle;
            }
            Physics.Raycast(hitInfo.point, secondDirection, out secondHit, float.MaxValue, secondArcMask);
            if (secondHit.collider == null) {
                Debug.LogError("MISSED THEGROUND!!");
                return;
            }
            int layerOfSecondHit = secondHit.collider.gameObject.layer;

            apex.transform.position = hitInfo.point;
            if (layerOfSecondHit == LayerMask.NameToLayer("Ground")) groundLocation.position = secondHit.point;
            else if (layerOfSecondHit == LayerMask.NameToLayer("EnemyRange")) groundLocation.position = findOffsetPoint(secondHit.collider, secondHit.point);
        }
        else if (layerOfHit == LayerMask.NameToLayer("Ground")) {
            groundLocation.position = hitInfo.point;
            placeStraightApex();
        }
        else if (layerOfHit == LayerMask.NameToLayer("EnemyRange")) {
            groundLocation.position = findOffsetPoint(hitInfo.collider, hitInfo.point);
            placeStraightApex();
        }   
    }

    void placeStraightApex() {
        Vector3 apexDispacement = (groundLocation.position - teleLineSpawn.position)/2;
        apex.position = teleLineSpawn.position + apexDispacement;
    }

    void setSmoothedPoints() {
        Vector3[] linePositions = new Vector3[3];
        for (int i = 0; i < 3; i ++) {
            linePositions[i] = linePoints[i].position;
        }
        Vector3[] smoothedPoints = LineSmoother.SmoothLine(linePositions, lineSegmentSize);

        teleportArc.numPositions = smoothedPoints.Length;
        teleportArc.SetPositions(smoothedPoints);
    }

    public void go() {
        if (teleporting || !active) return;
        StartCoroutine("TeleportPosition");
    }

    IEnumerator TeleportPosition() {
        teleporting = true;
        fadeOut();
        avatar.SetActive(false); // otherwise we see hands in the black while teleporting

        yield return new WaitForSeconds(FADE_DURATION);

        avatar.SetActive(true);
        Vector3 newPosition = new Vector3(groundLocation.position.x, player.transform.position.y, groundLocation.position.z);
        player.transform.position = newPosition;
        player.transform.forward = groundLocation.forward;
        fadeIn();
        teleporting = false;
    }

    public Vector3 findOffsetPoint(Collider collider, Vector3 hitLocation) {
        Vector3 direction = hitLocation - collider.transform.position;
        Vector3.ProjectOnPlane(direction, Vector3.up);
        direction.Normalize();
        direction = direction * (groundSphereCollider.radius);
        hitLocation = hitLocation + direction;
        RaycastHit hitInfo = new RaycastHit();
        Physics.Raycast(hitLocation, -Vector3.up, out hitInfo, float.MaxValue, LayerMask.GetMask(new string[1] { "Ground" }));
        return hitInfo.point;
    }

    public void setRotation(Vector2 vector) {
        groundLocation.forward = Vector3.ProjectOnPlane(teleLineSpawn.forward, Vector3.up); // TODO robust for hills (use normal)
        float angle = Mathf.Atan2(vector.x, vector.y) * Mathf.Rad2Deg;
        groundLocation.RotateAround(groundLocation.position, Vector3.up, angle);
        active = true;
    }

    public void disable() {
        active = false;
    }

    private void fadeOut() {
        fader.SetActive(true);
    }

    private void fadeIn() {
        fader.SetActive(false);
    }
}
