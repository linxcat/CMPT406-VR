using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRunner : Enemy{

    //GameObject weapon;
    //Transform pivot;
	private float detectRange = 100;
	private float atkRange = 1.5F;
    private float atkDuration = 1;
    private float atkCD = 0.5F;
    private float speed = 1.5F;
    private int turnSpeed = 3;
    private int attackDmg = 30;
    private int searchAngle = 80;
	private bool isAttack;
    float parryTime = 5F;

    //Vector3 weaponStartPosition;

    //float swingSpeed = 1.75F; //note, the flips in swinging must be > swingSpeed/2
    //bool swingDown = true;

	enum runnerState{
		idle,
		follow,
		attack,
		dead
	};

	private runnerState currentState;

	// Use this for initialization
	new void Start () {
        base.Start();
        //foreach (Transform child in transform)
        //{
        //    if (child.name == "weapon") {
        //        weapon = child.gameObject;
        //        weaponStartPosition = child.localPosition;
        //    }
        //    if (child.name == "pivot") {
        //        pivot = child.transform;
        //    }
        //    if (child.name == "model") colourMaterial = child.GetComponent<Renderer>().material;
        //}
		isAttack = false;
		currentState = runnerState.idle;
		player = GameObject.FindGameObjectWithTag ("Player");
	}
	
	// Update is called once per frame
	void Update () {
        if (!isAlive())
            currentState = runnerState.dead;
        switch (currentState) {
            case runnerState.idle:
                if (this.GetComponent<Animator>().GetInteger("state") != 4) {
                    this.GetComponent<Animator>().SetInteger("state", 4);
                }
			searchPlayer ();
			break;
            case runnerState.follow:
                if (this.GetComponent<Animator>().GetInteger("state") != 3) {
                    this.GetComponent<Animator>().SetInteger("state", 3);
                }
                moveTowardsPlayer ();
			break;
		case runnerState.attack:
			if (!isAttack) {
                    this.GetComponent<Animator>().SetInteger("state", 1);
                    isAttack = true;
				StartCoroutine ("swingWeapon");
			}
			break;
		case runnerState.dead:
                
                this.GetComponent<Animator>().SetInteger("state", 2);
                break;
		}
	}

    IEnumerator swingWeapon()
    {
        //tune atkDelay to match animation
        yield return new WaitForSeconds(atkDuration);
        this.GetComponent<Animator>().SetInteger("state", 4);
        yield return new WaitForSeconds(atkCD);
        isAttack = false;
		currentState = runnerState.follow;
    }

    public void counter() {
        if (!isAttack) return;

        //weapon.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
        //weapon.transform.up = pivot.up;
        //weapon.transform.localPosition = weaponStartPosition;
        //swingDown = true;
        StopCoroutine("swingWeapon");
        StartCoroutine("parried");
    }

    IEnumerator parried()
    {
        this.GetComponent<Animator>().SetInteger("state", 4);
        yield return new WaitForSeconds(parryTime);
        currentState = runnerState.idle;
        isAttack = false;
        //weapon.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
    }


    private void searchPlayer(){
		float angle = Vector3.Angle (player.transform.position - this.transform.position, this.transform.forward);
		float distance = Vector3.Distance (this.transform.position, player.transform.position);
        //Debug.Log("Angle: " + angle + "Distance: " + distance);
		if (angle < searchAngle && distance < detectRange) {
			currentState = runnerState.follow;
		}
	}

    private void moveTowardsPlayer() {
        float step = speed * Time.deltaTime;
        //Horizontal angle between this and player
        float angle = Vector3.Angle(new Vector3(player.transform.position.x, this.transform.position.y, player.transform.position.z)- this.transform.position, this.transform.forward);
        //Debug.Log("Angle: "+ angle);
        if (angle > 5)
        {
            Vector3 lookPos = player.transform.position - transform.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * turnSpeed);
        }
        else {
            Vector3 newPos = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, newPos, step);
            //lock on player
            facePlayer(newPos);
            if (Vector3.Distance(this.transform.position, player.transform.position) < atkRange) {
                currentState = runnerState.attack;
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        //replace player with blade collider
        //need to get player hp
        int playerHp = 100; //we do not have a player yet
        if (other.gameObject == GameObject.Find("Sword").gameObject) {
            //Damage is now determined in hit accuracy
            //takeDamage(20);
            //this.publish(new GUIPubSub.GUIEvent("health", playerHp - attackDmg));
        }
    }


}
