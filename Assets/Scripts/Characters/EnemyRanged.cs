using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRanged : Enemy{

    public GameObject projectile;
	private float detectRange = 100; //tuning required
	private float backupRange = 15;
	private float atkRange = 20;
	private float atkDelay = 5;
	private float speed = 2.5F;
    private int turnSpeed = 4;
    private int attackDmg = 30;
    private int searchAngle = 80;
    private bool isAttack;

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
        base.Start();
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
        if (!isAlive())
            currentState = rangedState.dead;
		switch (currentState) {
		case rangedState.idle:
			searchPlayer ();
                this.GetComponent<Animation>().Play("stand_vigilance");
			break;
		case rangedState.follow:
			moveTowardsPlayer ();
                this.GetComponent<Animation>().Play("walk");
                break;
		case rangedState.backup:
			backup ();
                this.GetComponent<Animation>().Play("walk");
                break;
		case rangedState.attack:
                if (!isAttack) {
                    isAttack = true;
                    StartCoroutine("fireProjectile");
                    this.GetComponent<Animation>().Play("attack01");
                }
			break;
		case rangedState.dead:
                this.GetComponent<Animation>().Play("dead");
                break;
        
		}
	}

	IEnumerator fireProjectile()
	{
        Invoke("spawnProjectile", 0.4f);
        yield return new WaitForSeconds(atkDelay);

        isAttack = false;
        if (Vector3.Distance(this.transform.position, player.transform.position) < backupRange) {
            currentState = rangedState.backup;
        }
        else {
            currentState = rangedState.follow;
        }

    }

    private void spawnProjectile() {
        GameObject x = (GameObject)Instantiate(projectile);
        x.transform.position = this.transform.position + new Vector3(0, 1f, 0);
    }

    private void searchPlayer()
    {

        //colourMaterial.SetColor("_Color", Color.white);
        float angle = Vector3.Angle(new Vector3(player.transform.position.x, this.transform.position.y, player.transform.position.z) - this.transform.position, this.transform.forward);
        float distance = Vector3.Distance(this.transform.position, player.transform.position);
        Debug.Log("Angle: " + angle + "Distance: " + distance);
        if (angle < searchAngle && distance < detectRange)
        {
            currentState = rangedState.follow;
        }
    }

    private void moveTowardsPlayer() {
        //colourMaterial.SetColor("_Color", Color.blue);
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
        //colourMaterial.SetColor("_Color", Color.green);
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

    private void OnTriggerEnter(Collider other) {
		//replace player with blade collider
		//need to get player hp
		int playerHp = 100; //we do not have a player yet
		if (other.gameObject == GameObject.Find("Sword").gameObject) {
			//takeDamage(20);
			//this.publish(new GUIPubSub.GUIEvent("health", playerHp - attackDmg));
		}
	}

}
