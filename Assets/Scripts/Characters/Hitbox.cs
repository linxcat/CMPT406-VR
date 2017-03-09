using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour {

    private CharacterStats playerStatManager;
    float invincibleTime = 2F;
    float timeCount;

	// Use this for initialization
	void Start () {
        playerStatManager = FindObjectOfType<CharacterStats>();
        timeCount = 0;
	}
	
	// Update is called once per frame
	void Update () {
        timeCount += Time.deltaTime;
	}

    public void getHit(int dmg){
        if (timeCount > invincibleTime){
            timeCount = 0;
            playerStatManager.takeDamage(dmg);
        }
    }
}
