using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour {

    public const float FADE_DURATION = 0.2f; // TODO change all constants to constant type
    private bool active = false;

    GameObject player;
    GameObject avatar;
    Transform apex;
    Transform groundLocation;

    // Use this for initialization
    void Start () {
        player = GameObject.Find("Player");
        apex = GameObject.Find("apex").transform;
        groundLocation = GameObject.Find("groundMarker").transform;
	}
	
	// Update is called once per frame
	void Update () {
		if (active) {
            // raycasting
        }
	}

    public void go() {
        StartCoroutine("TeleportPosition");
    }

    IEnumerator TeleportPosition() {
        fadeOut();
        avatar.SetActive(false); // otherwise we see hands in the black while teleporting
        yield return new WaitForSeconds(FADE_DURATION);
        avatar.SetActive(true);
        player.transform.position = groundLocation.position;
        player.transform.forward = groundLocation.forward;
        fadeIn();
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
