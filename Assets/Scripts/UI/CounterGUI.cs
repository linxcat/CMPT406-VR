using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterGUI : MonoBehaviour {

    Transform centerEyeAnchor;
    float oldTimeScale;
    static bool timeSlowEnded = false;

    // Use this for initialization
    void Start () {
        centerEyeAnchor = GameObject.Find("CenterEyeAnchor").transform;
        oldTimeScale = Time.timeScale;
        Debug.Log(oldTimeScale);
        Time.timeScale = 0.111111f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }
	
	// Update is called once per frame
	void Update () {
        transform.position = centerEyeAnchor.position + (centerEyeAnchor.forward * 0.6f) + (new Vector3(0, -0.0F, 0));
        transform.forward = (centerEyeAnchor.position - transform.position);
    }

    public void resetTimeSlow() {
        if (timeSlowEnded) {
            Time.timeScale = 1F;
            timeSlowEnded = false;
        }
        else Time.timeScale = oldTimeScale;
    }

    public float getOldTimeScale() {
        return oldTimeScale;
    }

    public static void endTimeSpell() {
        timeSlowEnded = true;
    }
}
