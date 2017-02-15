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
		if (active) {
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
        RaycastHit hitInfo = new RaycastHit();
        RaycastHit secondHit = new RaycastHit();

        if (!Physics.Raycast(teleLineSpawn.position, teleLineSpawn.forward, out hitInfo, float.MaxValue, teleportMask)) Debug.Log("HOLE IN TELEPORT BOUNDARIES!!");
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
            Debug.DrawRay(hitInfo.point, secondDirection, Color.red, 1F);
            if (!Physics.Raycast(hitInfo.point, secondDirection, out secondHit, float.MaxValue, secondArcMask)) Debug.Log("MISSED THEGROUND!!");
            int layerOfSecondHit = secondHit.collider.gameObject.layer;

            apex.transform.position = hitInfo.point;
            if (layerOfSecondHit == LayerMask.NameToLayer("Ground")) groundLocation.position = secondHit.point;
            else if (layerOfSecondHit == LayerMask.NameToLayer("EnemyRange")) groundLocation.position = findOffsetPoint(secondHit.collider, secondHit.transform.position);
        }
        else if (layerOfHit == LayerMask.NameToLayer("Ground")) {
            groundLocation.position = hitInfo.point;
            placeStraightApex();
        }
        else if (layerOfHit == LayerMask.NameToLayer("EnemyRange")) {
            groundLocation.position = findOffsetPoint(hitInfo.collider, hitInfo.transform.position);
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
        player.transform.position = newPosition; // TODO robust for hills
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
        float angle = Mathf.Atan2(vector.x, vector.y) * Mathf.Rad2Deg;
        groundLocation.RotateAround(groundLocation.position, Vector3.up, angle);
        active = true;
    }

    public void disable() {
        active = false;
    }

    private void fadeOut() {
        fader.SetActive(false);
    }

    private void fadeIn() {
        fader.SetActive(true);
    }
}
