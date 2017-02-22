using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRanged : MonoBehaviour{

    Material colourMaterial;
    GameObject projectile;
	public int hp = 100;
	private float detectRange = 100; //tunning required
	private float backupRange = 20;
	private float atkRange = 20;
	private float atkDelay = 5;
	private int speed = 3;
    private int turnSpeed = 3;
    private int attackDmg = 30;
    private int searchAngle = 80;
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
        foreach (Transform child in transform)
        {
            if (child.name == "model") colourMaterial = child.GetComponent<Renderer>().material;
        }
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
                StartCoroutine("fireProjectile");
			}
			break;
		case rangedState.dead:
			break;
		}
	}

	IEnumerator fireProjectile()
	{
        //TODO
        //Instantiate new projectile object
        colourMaterial.SetColor("_Color", Color.red);
        yield return new WaitForSeconds(atkDelay);

        isAttack = false;
		if (Vector3.Distance (this.transform.position, player.transform.position) <= backupRange)
			currentState = rangedState.backup;
		else
			currentState = rangedState.follow;
	}

    private void searchPlayer()
    {

        colourMaterial.SetColor("_Color", Color.white);
        float angle = Vector3.Angle(new Vector3(player.transform.position.x, this.transform.position.y, player.transform.position.z) - this.transform.position, this.transform.forward);
        float distance = Vector3.Distance(this.transform.position, player.transform.position);
        Debug.Log("Angle: " + angle + "Distance: " + distance);
        if (angle < searchAngle && distance < detectRange)
        {
            currentState = rangedState.follow;
        }
    }

    private void moveTowardsPlayer() {
        colourMaterial.SetColor("_Color", Color.blue);
        float step = speed * Time.deltaTime;
        //Horizontal angle between this and player
        float angle = Vector3.Angle(new Vector3(player.transform.position.x, this.transform.position.y, player.transform.position.z) - this.transform.position, this.transform.forward);
        Debug.Log("Angle: " + angle);
        if (angle > 5)
        {
            Vector3 lookPos = player.transform.position - transform.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * turnSpeed);
        }
        else
        {
            Vector3 newPos = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, newPos, step);
            //lock on player
            facePlayer(newPos);
            if (Vector3.Distance(this.transform.position, player.transform.position) < atkRange)
            {
                currentState = rangedState.attack;
            }
        }
    }

	private void backup(){
        colourMaterial.SetColor("_Color", Color.green);
        float step = speed * Time.deltaTime;
        //Horizontal angle between this and player
        float angle = Vector3.Angle(new Vector3(player.transform.position.x, this.transform.position.y, player.transform.position.z) - this.transform.position, this.transform.forward);
        Debug.Log("Angle: " + angle);
        if (angle < 175)
        {
            Vector3 lookPos = transform.position - player.transform.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * turnSpeed);
        }
        else
        {
            Vector3 newPos = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, newPos, -step);
            //lock on player
            //facePlayer(newPos);
            if (Vector3.Distance(this.transform.position, player.transform.position) > backupRange)
            {
                currentState = rangedState.follow;
            }
        }
    }

    private void facePlayer(Vector3 other)
    {
        transform.LookAt(other);
    }

    private void OnTriggerEnter(Collider other) {
		//replace player with blade collider
		//need to get player hp
		int playerHp = 100; //we do not have a player yet
		if (other.gameObject == GameObject.Find("Sword").gameObject) {
			takeDamage(20);
			//this.publish(new GUIPubSub.GUIEvent("health", playerHp - attackDmg));
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
