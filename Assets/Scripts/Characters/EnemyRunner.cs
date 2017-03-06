using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRunner : Enemy{

	private float detectRange = 100;
	private float atkRange = 1.2F;
    private float atkDuration = 1.5F;
    private float atkCD = 1.5F;
    private float speed = 2F;
    private int attackDmg = 30;
    private int searchAngle = 180;
    float parryTime = 5F;

    bool attacking = false;
    bool parryable = false;

    Animator anim;

	enum runnerState{
		idle,
		follow,
		attack,
		dead
	};

	private runnerState currentState = runnerState.idle;

	// Use this for initialization
	void Start () {
        base.Start();
        atkRange = transform.FindChild("Range").GetComponent<CapsuleCollider>().radius;
        anim = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {

        switch (currentState) {
            case runnerState.dead:
                break;
            case runnerState.idle:
			    searchPlayer ();
			    break;
            case runnerState.follow:
                moveTowardsPlayer();
                attackCheck();
			    break;
            case runnerState.attack:
                if (!attacking) StartCoroutine("attack");
                break;
		}
	}

    public override void swingHit(Hit hit) {
        switch (hit.getAccuracy()) {
            case Hit.ACCURACY.Perfect:
                audioSource.PlayOneShot(perfectHitClip, 0.2f);
                takeDamage(maxDamage);
                Debug.Log("Enemy hit! Damage: " + maxDamage);
                break;
            case Hit.ACCURACY.Good:
                audioSource.PlayOneShot(goodHitClip, 0.2f);
                takeDamage(maxDamage / 2);
                Debug.Log("Enemy hit! Damage: " + maxDamage / 2);
                break;
            case Hit.ACCURACY.Bad:
                audioSource.PlayOneShot(badHitClip, 0.2f);
                takeDamage(maxDamage / 4);
                Debug.Log("Enemy hit! Damage: " + maxDamage / 4);
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

        if (angle > 5) {
            slowFacePlayer();
        }
        else {
            anim.SetBool("moving", true);
            move();
            fall();
            attackCheck();
        }
    }

    void move() {
        float step = speed * Time.deltaTime;
        Vector3 targetPos = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPos, step);
    }
    
    void fall() {
        RaycastHit hitPoint = new RaycastHit();
        Vector3 aboveGround = transform.position;
        aboveGround.y += 1F;
        Physics.Raycast(aboveGround, Vector3.down, out hitPoint, float.MaxValue, LayerMask.GetMask(new string[] {"Ground"}));
        transform.position = hitPoint.point;
    }

    IEnumerator attack() {
        anim.SetBool("moving", false);
        anim.SetTrigger("attack");
        attacking = true;
        parryable = true;
        yield return new WaitForSeconds(atkDuration);
        parryable = false;
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
        currentState = runnerState.idle;
    }

    void attackCheck() {
        if (Vector3.Distance(transform.position, player.transform.position) < atkRange) {
            currentState = runnerState.attack;
            facePlayer();
        }
    }

    public override void die() {
        GetComponent<Animator>().SetTrigger("kill");
        StopAllCoroutines();
        currentState = runnerState.dead;
        GetComponent<Collider>().enabled = false;
    }

	public int getAtkDmg(){
		return attackDmg;
	}
}
