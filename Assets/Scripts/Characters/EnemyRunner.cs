using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRunner : Enemy{

    private float detectRange = 100;
    private float atkRange;
    private float atkWindUp = 0.5F;
    private float atkDuration = 1F;
    private float atkCD = 4F;
    private float speed = 2F;
    private int attackDmg = 20;
    private int searchAngle = 180;
    private float spawnTimer = 3.5F;
    private float spawnRoarDelay = 2F;
    float parryTime = 5F;
    float HEIGHTBIAS = 0F;

    public AudioClip runnerRoarClip;

    bool spawning = false;
    bool attacking = false;
    bool parryable = false;

    public AudioClip hitAttack;
    public AudioClip beingHit;
    public AudioClip deathSound;


    Animator anim;

    enum runnerState{
        spawning,
        idle,
        follow,
        attack,
        dead
    };

    private runnerState currentState = runnerState.spawning;

	// Use this for initialization
	void Start () {
        base.Start();
        atkRange = transform.FindChild("Range").GetComponent<CapsuleCollider>().radius + player.GetComponent<CapsuleCollider>().radius;
        anim = GetComponent<Animator>();
        agent.SetDestination (transform.position);
        agent.Stop ();
    }
	
	// Update is called once per frame
	void Update () {

        switch (currentState) {
			case runnerState.spawning:
				if (!spawning) StartCoroutine ("spawn");
				break;
            case runnerState.idle:
			    searchPlayer ();
			    break;
            case runnerState.follow:
                moveTowardsPlayer();
			    break;
            case runnerState.attack:
                if (!attacking) StartCoroutine("attack");
                break;
		}
	}

    public override void swingHit(Hit hit) {
        audioSource.PlayOneShot(beingHit, 0.2f);
        switch (hit.getAccuracy()) {
            case Hit.ACCURACY.Perfect:
                audioSource.PlayOneShot(perfectHitClip, 0.2f);
                takeDamage(maxDamage);
                break;
            case Hit.ACCURACY.Good:
                audioSource.PlayOneShot(goodHitClip, 0.2f);
                takeDamage(maxDamage / 2);
                break;
            case Hit.ACCURACY.Bad:
                audioSource.PlayOneShot(badHitClip, 0.2f);
                takeDamage(maxDamage / 4);
                break;
        }
    }

    private void searchPlayer() {
        anim.SetBool("moving", false);
        float angle = Vector3.Angle(player.transform.position - transform.position, transform.forward);
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (angle < searchAngle && distance < detectRange) {
            currentState = runnerState.follow;
        }
        fall();
    }

    private void moveTowardsPlayer() {
        Vector3 axisRotate = Vector3.ProjectOnPlane(player.transform.position - transform.position, Vector3.up);
        float angle = Vector3.Angle(axisRotate, transform.forward);

        //if (angle > 5) {
        //    slowFacePlayer();
        //}
        //else {
            anim.SetBool("moving", true);
            //move();
            agent.Resume();
            agent.destination = player.transform.position;
            fall();
            attackCheck();
        //}
    }

    //void move() {
    //    float step = speed * Time.deltaTime;
    //    Vector3 targetPos = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
    //    transform.position = Vector3.MoveTowards(transform.position, targetPos, step);
    //}
    
    void fall() {
        RaycastHit hitPoint = new RaycastHit();
        Vector3 aboveGround = transform.position;
        aboveGround.y += 1F;
        Physics.Raycast(aboveGround, Vector3.down, out hitPoint, float.MaxValue, LayerMask.GetMask(new string[] {"Ground"}));
        Vector3 groundPoint = hitPoint.point;
        groundPoint.y -= HEIGHTBIAS;
        transform.position = groundPoint;
    }

	IEnumerator spawn(){
		spawning = true;
		yield return new WaitForSeconds (spawnRoarDelay);
		audioSource.PlayOneShot(runnerRoarClip);
		yield return new WaitForSeconds (spawnTimer - spawnRoarDelay);
		currentState = runnerState.idle;
	}

    IEnumerator attack() {
        anim.SetBool("moving", false);
        agent.Stop();
        anim.SetTrigger("attack");
        audioSource.PlayOneShot(hitAttack, 0.2f);
        attacking = true;
        
        yield return new WaitForSeconds(atkWindUp);
        parryable = true;
        yield return new WaitForSeconds(atkDuration);
        parryable = false;
        if(!attackCheck())
            currentState = runnerState.idle;
        yield return new WaitForSeconds(atkCD);
        attacking = false;
    }

    public override void counter() {
        if (!parryable) return;
        StartCoroutine("parry");
    }

    IEnumerator parry()
    {
        anim.SetTrigger("counter");
        StopCoroutine("attack");
        attacking = false;
        parryable = false;
        yield return new WaitForSeconds(parryTime);
        if (!attackCheck())
            currentState = runnerState.idle;
    }

    bool attackCheck() {
        Vector3 temp = new Vector3(transform.position.x, player.transform.position.y, transform.position.z);
        //Debug.Log(Vector3.Distance(temp, player.transform.position));
        if (Vector3.Distance(temp, player.transform.position) <= atkRange) {
            currentState = runnerState.attack;
            facePlayer();
            return true;
        }
        return false;
    }

    public override void die() {
        audioSource.PlayOneShot(deathSound, 0.2f);
        GetComponent<Animator>().SetTrigger("kill");
        StopAllCoroutines();
        currentState = runnerState.dead;
        GetComponent<Collider>().enabled = false;
    }

	public int getAtkDmg(){
		return attackDmg;
	}

    public bool isParriable(){
        return parryable;
    }
}
