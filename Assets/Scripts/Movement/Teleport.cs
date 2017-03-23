using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
    static float bumpBack;
    int teleportMask;
    int secondArcMask;

    AudioSource audioSource;
    public AudioClip teleportStartClip;
    public AudioClip teleportExecuteClip;

    // Use this for initialization
    void Start () {
        player = GameObject.Find("Player");
        avatar = GameObject.Find("LocalAvatar");
        fader = GameObject.Find("Fader");
        fader.SetActive(false);
        teleportArc = GetComponent<LineRenderer>();
        apex = GameObject.Find("apex").transform;
        groundLocation = GameObject.Find("groundMarker").transform;
        groundSphereCollider = groundLocation.GetComponent<SphereCollider>();
        bumpBack = groundSphereCollider.radius;
        teleportMask = LayerMask.GetMask(new string[4] { "Ground", "EnemyRange", "TeleportCollider", "Walls" });
        secondArcMask = LayerMask.GetMask(new string[3] { "Ground", "EnemyRange", "Walls" });

        linePoints[0] = null; // not dynamic pointer, need updating
        linePoints[1] = apex;
        linePoints[2] = groundLocation;

        audioSource = GetComponent<AudioSource>();
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
                float angle = 45;
                Vector3 appropriateAxis;

                float handRoll = teleLineSpawn.rotation.eulerAngles.z;
                Vector3 flatForward = Vector3.ProjectOnPlane(teleLineSpawn.forward, Vector3.up);

                GameObject toy = Instantiate(teleLineSpawn.gameObject, teleLineSpawn.position, teleLineSpawn.localRotation, teleLineSpawn.parent);
                toy.transform.rotation *= Quaternion.LookRotation(flatForward, teleLineSpawn.transform.up);
                float upAmount = Vector3.Project(toy.transform.up, Vector3.up).magnitude;
                float rightAmount = Vector3.Project(toy.transform.right, Vector3.up).magnitude;

                if (rightAmount < upAmount) {
                    appropriateAxis = toy.transform.right;
                    if (handRoll >= 90 && handRoll <= 270) angle = -angle;
                }
                else {
                    appropriateAxis = toy.transform.up;
                    if (handRoll > 0 && handRoll < 180) angle = -angle;
                }
                Destroy(toy);
                Vector3 flattenedHandAxis = Vector3.ProjectOnPlane(appropriateAxis, Vector3.up);
                
                secondDirection = Quaternion.AngleAxis(angle, flattenedHandAxis) * flatForward;
            }

            Physics.Raycast(hitInfo.point, secondDirection, out secondHit, float.MaxValue, secondArcMask);
            if (secondHit.collider == null) {
                Debug.LogError("MISSED THEGROUND!!");
                return;
            }
            int layerOfSecondHit = secondHit.collider.gameObject.layer;

            apex.transform.position = hitInfo.point;
            if (layerOfSecondHit == LayerMask.NameToLayer("Ground")) placeOnGround(secondHit.point);
            else if (layerOfSecondHit == LayerMask.NameToLayer("EnemyRange")) groundLocation.position = findOffsetPoint(secondHit.collider, secondHit.point);
            else if (layerOfSecondHit == LayerMask.NameToLayer("Walls")) groundLocation.position = wallBounce(secondHit.normal, secondHit.point);
        }
        else if (layerOfHit == LayerMask.NameToLayer("Ground")) {
            placeOnGround(hitInfo.point);
            placeStraightApex();
        }
        else if (layerOfHit == LayerMask.NameToLayer("EnemyRange")) {
            groundLocation.position = findOffsetPoint(hitInfo.collider, hitInfo.point);
            placeStraightApex();
        }
        else if (layerOfHit == LayerMask.NameToLayer("Walls")) {
            groundLocation.position = wallBounce(hitInfo.normal, hitInfo.point);
            placeStraightApex();
        }
    }

    void placeStraightApex() {
        Vector3 apexDispacement = (groundLocation.position - teleLineSpawn.position)/2;
        apex.position = teleLineSpawn.position + apexDispacement;
    }

    void setSmoothedPoints() {
        linePoints[0] = teleLineSpawn;
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
        audioSource.PlayOneShot(teleportExecuteClip, 0.2f);
        teleporting = true;
        fadeOut();
        avatar.SetActive(false); // otherwise we see hands in the black while teleporting

        yield return new WaitForSecondsRealtime(FADE_DURATION);

        avatar.SetActive(true);
        player.transform.position = groundLocation.position;
        player.transform.forward = groundLocation.forward;
        groundLocation.position = player.transform.position; //childed objects get shoved forward, avoid inside walls
        apex.position = player.transform.position;
        fadeIn();
        teleporting = false;
    }

    Vector3 findOffsetPoint(Collider collider, Vector3 hitLocation) {
        Vector3 direction = hitLocation - collider.transform.position;
        direction = Vector3.ProjectOnPlane(direction, Vector3.up);
        direction.Normalize();
        float bumpTotal = bumpBack + ((CapsuleCollider)collider).radius;
        direction = direction * (bumpTotal);
        Vector3 newPosition = collider.transform.position + direction;
        return groundCast(newPosition);
    }

    Vector3 wallBounce(Vector3 normal, Vector3 contact) {
        Vector3 direction = normal.normalized * bumpBack;
        Vector3 spot = contact + direction;
        return groundCast(spot);
    }

    Vector3 groundCast(Vector3 startPoint) {
        Vector3 higherPoint = startPoint;
        higherPoint.y += 1;
        RaycastHit hitInfo = new RaycastHit();
        Physics.Raycast(higherPoint, -Vector3.up, out hitInfo, float.MaxValue, LayerMask.GetMask(new string[1] { "Ground" }));
        return hitInfo.point;
    }

    private void placeOnGround(Vector3 spot) {
        Collider[] possibleObstacles = Physics.OverlapSphere(spot, groundSphereCollider.radius, LayerMask.GetMask(new string[1] { "EnemyRange" }));

        if (possibleObstacles.Length == 0) onNavMesh(spot);
        else {
            Vector3[] safePoints = new Vector3[possibleObstacles.Length];
            for (int i = 0; i < possibleObstacles.Length; i ++) {
                Vector3 closestPoint = possibleObstacles[i].ClosestPointOnBounds(spot);
                if (possibleObstacles[i].gameObject.layer == LayerMask.NameToLayer("EnemyRange")) {
                    safePoints[i] = findOffsetPoint(possibleObstacles[i], closestPoint);
                }
                else if (possibleObstacles[i].gameObject.layer == LayerMask.NameToLayer("Walls")) {
                    closestPoint.y += 0.1F; // otherwise cast happens at ground level and shoots through the floor
                    Ray ray = new Ray(spot, closestPoint);
                    RaycastHit hitInfo;
                    possibleObstacles[i].Raycast(ray, out hitInfo, float.MaxValue);
                    safePoints[i] = wallBounce(hitInfo.normal, closestPoint);
                }
            }

            // PLACEHOLDER (TODO)
            if (safePoints.Length == 1) {
                onNavMesh(safePoints[0]);
            }
            else {
                Debug.LogError(" HARD FLOOR PLACEMENT CASE!!");
                disable();
            }
        }

    }

    void onNavMesh(Vector3 point) {
        NavMeshHit hitPoint;

        if (!NavMesh.Raycast(player.transform.position, point, out hitPoint, NavMesh.AllAreas)) {
            groundLocation.position = point;
        }
        else {
            NavMeshHit newLocation;
            NavMesh.SamplePosition(hitPoint.position, out newLocation, 0.1F, NavMesh.AllAreas);
        }
        
    }

    public void setRotation(Vector2 vector) {
        groundLocation.forward = Vector3.ProjectOnPlane(teleLineSpawn.forward, groundLocation.up); // TODO robust for hills (use normal)
        float angle = Mathf.Atan2(vector.x, vector.y) * Mathf.Rad2Deg;
        groundLocation.RotateAround(groundLocation.position, groundLocation.up, angle);
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

    public void setTeleLineSpawn(Transform value) {
        teleLineSpawn = value;
    }
}
