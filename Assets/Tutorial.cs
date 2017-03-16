using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour {

    float leaveCount;

	// Use this for initialization
	void Start () {
        leaveCount = 0f;
	}
	
	// Update is called once per frame
	void Update () {
		if(OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger)) {
            leaveCount += Time.deltaTime;
            if(leaveCount > 2.0f) {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
        else {
            leaveCount = 0f;
        }
	}
}
