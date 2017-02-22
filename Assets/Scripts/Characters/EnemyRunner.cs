using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRunner : GUIPubSub.GUIPublisher {

    Material colourMaterial;
    GameObject weapon;
    Transform pivot;
    public int hp = 100;
	public float detectRange = 1000;
	public float atkRange = 5;
    private int speed = 3;
    private int attackDmg = 30;
	private bool isAttack;
    GameObject player;

    Vector3 weaponStartPosition;

    float swingSpeed = 1.75F; //note, the flips in swinging must be > swingSpeed/2
    bool swingDown = true;

    float parryTime = 2F;

	enum runnerState{
		idle,
		follow,
		attack,
		dead
	};

	private runnerState currentState;

	// Use this for initialization
	void Start () {
        foreach (Transform child in transform)
        {
            if (child.name == "weapon") {
                weapon = child.gameObject;
                weaponStartPosition = child.localPosition;
            }
            if (child.name == "pivot") {
                pivot = child.transform;
            }
            if (child.name == "model") colourMaterial = child.GetComponent<Renderer>().material;
        }
		isAttack = false;
		currentState = runnerState.idle;
		player = GameObject.FindGameObjectWithTag ("Player");
        //StartCoroutine("swingWeapon", 0F);
	}
	
	// Update is called once per frame
	void Update () {
		switch (currentState) {
		case runnerState.idle:
			searchPlayer ();
			break;
		case runnerState.follow:
			moveTowardsPlayer ();
			break;
		case runnerState.attack:
			if (!isAttack) {
				isAttack = true;
				StartCoroutine ("swingWeapon", 0F);
			}
			break;
		case runnerState.dead:
			break;
		}
	}

    public void swingHit(Hit hit) {
        StopCoroutine("colourFlash");
        StartCoroutine("colourFlash", hit.getAccuracy());
    }

    IEnumerator colourFlash(Hit.ACCURACY accuracy) {
        switch (accuracy) {
            case Hit.ACCURACY.Perfect:
                colourMaterial.SetColor("_Color", Color.blue);
                break;
            case Hit.ACCURACY.Good:
                colourMaterial.SetColor("_Color", Color.green);
                break;
            case Hit.ACCURACY.Bad:
                colourMaterial.SetColor("_Color", Color.red);
                break;
        }
        yield return new WaitForSeconds(0.75F);
        colourMaterial.SetColor("_Color", Color.white);
    }

    IEnumerator swingWeapon(float delay)
    {
        yield return new WaitForSeconds(delay);
        weapon.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
		int count = 1;

		while (count>0)
        {
            if (swingDown)
            {
                weapon.transform.RotateAround(pivot.position, pivot.right, swingSpeed);
            }
            else
            {
                weapon.transform.RotateAround(pivot.position, pivot.right, -swingSpeed);
            }
			if (swingDown && (Vector3.Angle (weapon.transform.up, pivot.forward) < 1F))
				swingDown = false;
			else if (!swingDown && (Vector3.Angle (weapon.transform.up, pivot.up) < 1F)) {
				swingDown = true;
				count++;
			}

            yield return null;
        }
		isAttack = false;
		currentState = runnerState.follow;
    }

    public void counter() {
        if (!swingDown) return;

        weapon.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
        weapon.transform.up = pivot.up;
        weapon.transform.localPosition = weaponStartPosition;
        swingDown = true;
        StopCoroutine("swingWeapon");
        //StartCoroutine("swingWeapon", parryTime);
    }

	private void searchPlayer(){
		float angle = Vector3.Angle (player.transform.position - this.transform.position, this.transform.forward);
		float distance = Vector3.Distance (this.transform.position, player.transform.position);
		if (angle < 90 && distance < detectRange) {
			currentState = runnerState.follow;
		}
	}

    private void moveTowardsPlayer() {
        float step = speed * Time.deltaTime;

        // this if statement will stop enemies moving withing 5 units of the player
        //if (Vector3.Distance(this.transform.position, player.transform.position) > 5){
		facePlayer();
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, step);
        //}

		if (Vector3.Distance (this.transform.position, player.transform.position) < atkRange) {
			currentState = runnerState.attack;
		}
    }

    private void facePlayer() {
        //if (Vector3.Distance(this.transform.position, player.transform.position) > 5) {
		transform.LookAt(player.transform);
        //}
    }

    private void OnTriggerEnter(Collider other) {
        //replace player with blade collider
        //need to get player hp
        int playerHp = 100; //we do not have a player yet
        if (other.gameObject == GameObject.Find("Sword").gameObject) {
            takeDamage(20);
            this.publish(new GUIPubSub.GUIEvent("health", playerHp - attackDmg));
        }
    }

    private void takeDamage(int damage) {
        this.hp = this.hp - damage;
    }

    private bool isAlive() {
        if (hp <= 0) {
            return false;
        }
        else {
            return true;
        }
    }

    public int getHp() {
        return this.hp;
    }
}
