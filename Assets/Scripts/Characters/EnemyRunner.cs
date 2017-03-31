using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRunner : Enemy{

    private int runnerMaxHealth = 570;
    private SpawnManager spawnManager;
    private float detectRange = 100;
    private float atkRange;
    private float atkWindUp = 0.5F;
    private float atkDuration = 1F;
    private float atkCD = 4F;
    private float speed = 2F;
    private int attackDmg = 100;
    private int searchAngle = 180;
    private float spawnTimer = 3.5F;
    private float spawnRoarDelay = 2F;
    float parryTime = 5F;
    float HEIGHTBIAS = 0.1F;

    public AudioClip runnerRoarClip;

    public AudioSource walkingSource;

    bool spawning = false;
    bool attacking = false;
    bool parryable = false;

    public AudioClip hitAttack;
    public AudioClip beingHit;
    public AudioClip deathSound;

    // Haptics
    public AudioClip badHapticAudio;
    public AudioClip goodHapticAudio;
    public AudioClip perfectHapticAudio;
    OVRHapticsClip badHapticClip;
    OVRHapticsClip goodHapticClip;
    OVRHapticsClip perfectHapticClip;

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
    void Start() {
        base.Start();
        hp = runnerMaxHealth;
        spawnManager = FindObjectOfType<SpawnManager> ();
        atkRange = transform.FindChild("Range").GetComponent<CapsuleCollider>().radius + player.GetComponent<CapsuleCollider>().radius;
        anim = GetComponent<Animator>();
        agent.SetDestination (transform.position);
        agent.Stop();

        //Haptics
        badHapticClip = new OVRHapticsClip(badHapticAudio);
        goodHapticClip = new OVRHapticsClip(goodHapticAudio);
        perfectHapticClip = new OVRHapticsClip(perfectHapticAudio);
    }
	
	// Update is called once per frame
	void Update() {
        switch (currentState) {
			case runnerState.spawning:
				if (!spawning) StartCoroutine ("spawn");
				break;
            case runnerState.idle:
                walkingSource.Stop();
                searchPlayer();
			    break;
            case runnerState.follow:
                if (!walkingSource.isPlaying) walkingSource.Play();
                moveTowardsPlayer();
			    break;
            case runnerState.attack:
                walkingSource.Stop();
                if (!attacking) StartCoroutine("attack");
                break;
		}
	}

    public override void swingHit(Hit hit) {
        audioSource.PlayOneShot(beingHit);
        switch (hit.getAccuracy()) {
            case Hit.ACCURACY.Perfect:
                audioSource.PlayOneShot(perfectHitClip);
                InitiateHapticFeedback(perfectHapticClip, 1);
                takeDamage(perfectDamage);
                break;
            case Hit.ACCURACY.Good:
                audioSource.PlayOneShot(goodHitClip);
                InitiateHapticFeedback(goodHapticClip, 1);
                takeDamage(goodDamage);
                break;
            case Hit.ACCURACY.Bad:
                audioSource.PlayOneShot(badHitClip);
                InitiateHapticFeedback(badHapticClip, 1);
                takeDamage(badDamage);
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
        anim.SetBool("moving", true);
        agent.Resume();
        agent.destination = player.transform.position;
        fall();
        attackCheck();
    }
    
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
        attacking = true;
        StartCoroutine("attackOnCD");
        anim.SetBool("moving", false);
        agent.Stop();
        anim.SetTrigger("attack");
        audioSource.PlayOneShot(hitAttack);
        yield return new WaitForSeconds(atkWindUp);
        parryable = true;
        yield return new WaitForSeconds(atkDuration);
        parryable = false;
        if(!attackCheck())
            currentState = runnerState.idle;
    }

    IEnumerator attackOnCD()
    {
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
        parryable = false;
        yield return new WaitForSeconds(parryTime);
        if (!attackCheck())
            currentState = runnerState.idle;
    }

    bool attackCheck() {
        Vector3 temp = new Vector3(transform.position.x, player.transform.position.y, transform.position.z);
        if (Vector3.Distance(temp, player.transform.position) <= atkRange) {
            currentState = runnerState.attack;
            agent.Stop();
            facePlayer();
            walkingSource.Stop();
            return true;
        }
        return false;
    }

    public override void die() {
        audioSource.PlayOneShot(deathSound);
        GetComponent<Animator>().SetTrigger("kill");
        StopAllCoroutines();
        spawnManager.EnemyKilled();
        agent.enabled = false;
        currentState = runnerState.dead;
        GetComponent<Collider>().enabled = false;
        StartCoroutine("sink");
    }

    IEnumerator sink() {
        yield return new WaitForSeconds(5);
        for (int i = 0; i < 150; i ++) {
            Vector3 newPosition = transform.position;
            newPosition.y -= 0.005F;
            transform.position = newPosition;
            yield return 0;
        }
        Destroy(gameObject);
    }

	public int getAtkDmg(){
		return attackDmg;
	}

    public bool isParriable(){
        return parryable;
    }

}
