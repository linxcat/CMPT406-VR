using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour {

    float leaveCount = 0F;
    private CharacterStats characterStats;

    void Start(){
        characterStats = FindObjectOfType<CharacterStats> ();
    }
	
	// Update is called once per frame
	void Update () {
		if(OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger)) {
            leaveCount += Time.deltaTime;
            if(leaveCount > 2.0f) {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
        else {
            leaveCount = 0f;
        }
        characterStats.maxStamina;
	}
}
