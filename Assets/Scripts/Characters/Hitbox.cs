using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour {

    private CharacterStats playerStatManager;
    float timeCount;

	// Use this for initialization
	void Start () {
        playerStatManager = FindObjectOfType<CharacterStats>();
	}
	
	// Update is called once per frame
	void Update () {
        timeCount += Time.deltaTime;
	}

    public void getHit(int dmg){
        playerStatManager.takeDamage(dmg);
    }
}
