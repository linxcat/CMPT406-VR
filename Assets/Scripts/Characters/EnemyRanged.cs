using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRanged : GUIPubSub.GUIPublisher {


	GameObject projectile;
	public int hp = 100;
	public float detectRange = 1000; //tunning required
	public float backupRange = 100;
	public float atkRange = 500;
	public float atkDelay = 5;
	private int speed = 3;
	private int attackDmg = 30;
	private bool isAttack;
	GameObject player;

	enum rangedState{
		idle,
		follow,
		backup,
		attack,
		dead
	};

	private rangedState currentState;

	// Use this for initialization
	void Start () {
		//TODO
		//find projectile object

		isAttack = false;
		currentState = rangedState.idle;
		player = GameObject.FindGameObjectWithTag ("Player");
	}

	// Update is called once per frame
	void Update () {
		switch (currentState) {
		case rangedState.idle:
			searchPlayer ();
			break;
		case rangedState.follow:
			moveTowardsPlayer ();
			break;
		case rangedState.backup:
			backup ();
			break;
		case rangedState.attack:
			if (!isAttack) {
				isAttack = true;
				StartCoroutine ("fireProjectile");
			}
			break;
		case rangedState.dead:
			break;
		}
	}

	IEnumerator fireProjectile()
	{
		//TODO
		//Intensiate new projectile object
		yield return new WaitForSeconds(atkDelay);

		isAttack = false;
		if (Vector3.Distance (this.transform.position, player.transform.position) < backupRange)
			currentState = rangedState.backup;
		else
			currentState = rangedState.follow;
	}

	private void searchPlayer(){
		float angle = Vector3.Angle (player.transform.position - this.transform.position, this.transform.forward);
		float distance = Vector3.Distance (this.transform.position, player.transform.position);
		if (angle < 90 && distance < detectRange) {
			currentState = rangedState.follow;
		}
	}

	private void moveTowardsPlayer() {
		float step = speed * Time.deltaTime;

		// this if statement will stop enemies moving withing 5 units of the player
		//if (Vector3.Distance(this.transform.position, player.transform.position) > 5){
		facePlayer();
		Vector3 newPos = new Vector3 (player.transform.position.x, transform.position.y, player.transform.position.z);
		transform.position = Vector3.MoveTowards(transform.position, newPos, step);
		//}

		if (Vector3.Distance (this.transform.position, player.transform.position) < atkRange) {
			currentState = rangedState.attack;
		}
	}

	private void backup(){
		float step = -speed * Time.deltaTime;
		facePlayer();
		Vector3 newPos = new Vector3 (player.transform.position.x, transform.position.y, player.transform.position.z);
		transform.position = Vector3.MoveTowards(transform.position, newPos, step);
		//}

		if (Vector3.Distance (this.transform.position, player.transform.position) > backupRange) {
			currentState = rangedState.attack;
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
