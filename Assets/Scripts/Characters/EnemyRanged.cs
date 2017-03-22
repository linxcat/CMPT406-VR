﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRanged : Enemy {

    private SpawnManager spawnManager;
    public GameObject projectile;
    private float detectRange = 100; //tuning required
    private float backupRange = 10;
    private float atkRange = 15;
    private float atkDelay = 5;
    private float speed = 2.5F;
    private int attackDmg = 30;
    private int searchAngle = 180;
    private float spawnTimer = 3.5F;

    bool spawning = false;
    bool attacking = false;
    bool parryable = false;

    Animator anim;
    RaycastHit hit;
    LayerMask mask;

    // Haptics
    public AudioClip badHapticAudio;
    public AudioClip goodHapticAudio;
    public AudioClip perfectHapticAudio;
    OVRHapticsClip badHapticClip;
    OVRHapticsClip goodHapticClip;
    OVRHapticsClip perfectHapticClip;

    enum rangedState {
        spawn,
        idle,
        follow,
        //backup,
        attack,
        dead
    };

    private rangedState currentState = rangedState.spawn;

    // Use this for initialization
    void Start() {
        base.Start();
        hp = 50;
        spawnManager = FindObjectOfType<SpawnManager>();
        //TODO find projectile object
        turnSpeed = 4;
        anim = GetComponent<Animator>();
        mask = LayerMask.GetMask(new string[2] { "Player", "Ground" });

        //Haptics
        badHapticClip = new OVRHapticsClip(badHapticAudio);
        goodHapticClip = new OVRHapticsClip(goodHapticAudio);
        perfectHapticClip = new OVRHapticsClip(perfectHapticAudio);
    }

    // Update is called once per frame
    void Update() {
        switch (currentState) {
            case rangedState.idle:
                searchPlayer();
                break;
            case rangedState.spawn:
                if (!spawning) StartCoroutine("spawn");
                break;
            case rangedState.follow:
                moveTowardsPlayer();
                break;
            /*case rangedState.backup:
                facePlayer();
                backUp();
                break;*/
            case rangedState.attack:
                facePlayer();
                if (!attacking) StartCoroutine("fireProjectile");
                break;
        }
    }

    private void searchPlayer() {
        anim.SetBool("moving", false);
        currentState = rangedState.idle;

        float angle = Vector3.Angle(player.transform.position - transform.position, transform.forward);
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (angle < searchAngle && distance < detectRange)
            currentState = rangedState.follow;

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

    //void move(bool forward) {
    //    float step = speed * Time.deltaTime;
    //    if (!forward) step = -step;
    //    Vector3 targetPos = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
    //    transform.position = Vector3.MoveTowards(transform.position, targetPos, step);
    //}

    void fall() {
        RaycastHit hitPoint = new RaycastHit();
        Vector3 aboveGround = transform.position;
        aboveGround.y += 1F;
        Physics.Raycast(aboveGround, Vector3.down, out hitPoint, float.MaxValue, LayerMask.GetMask(new string[] { "Ground" }));
        transform.position = hitPoint.point;
    }

    IEnumerator spawn()
    {
        spawning = true;
        yield return new WaitForSeconds(spawnTimer);
        currentState = rangedState.idle;
    }

    /*
    private void backUp() {
        Vector3 axisRotate = Vector3.ProjectOnPlane(player.transform.position - transform.position, Vector3.up);
        float angle = Vector3.Angle(axisRotate, transform.forward);

        if (angle > 5) {
            slowFacePlayer();
        }
        else {
            anim.SetBool("moving", true);
            move(false);
            fall();
        }
    }*/

    bool attackCheck()
    {
        Vector3 temp = new Vector3(transform.position.x, player.transform.position.y, transform.position.z);
        //Debug.Log(Vector3.Distance(temp, player.transform.position));
        if (Vector3.Distance(temp, player.transform.position) <= atkRange &&
            Physics.Raycast(transform.position, transform.forward, out hit, float.MaxValue, mask))
        {
            if (hit.collider.name == "Hitbox") {
                currentState = rangedState.attack;
                facePlayer();
                return true;
            }
        }
        return false;
    }

    IEnumerator fireProjectile() {
        agent.Stop();
        anim.SetBool("moving", false);
        anim.SetTrigger("attack");
        attacking = true;
        parryable = true;
        Invoke("spawnProjectile", 0.4f);
        yield return new WaitForSeconds(atkDelay);

        attacking = false;
        parryable = false;
        if(!attackCheck())
            currentState = rangedState.idle;

    }

    private void spawnProjectile() {
        GameObject x = (GameObject)Instantiate(projectile);
        x.transform.position = transform.position + new Vector3(0, 1f, 0);
        x.GetComponent<Projectile>().setTarget(player);
        x.GetComponent<Projectile>().setOriginator(gameObject);
    }

    public override void swingHit(Hit hit) {
        switch (hit.getAccuracy()) {
            case Hit.ACCURACY.Perfect:
                audioSource.PlayOneShot(perfectHitClip, 0.2f);
                InitiateHapticFeedback(perfectHapticClip, 1);
                takeDamage(maxDamage);
                break;
            case Hit.ACCURACY.Good:
                audioSource.PlayOneShot(goodHitClip, 0.2f);
                InitiateHapticFeedback(goodHapticClip, 1);
                takeDamage(maxDamage / 2);
                break;
            case Hit.ACCURACY.Bad:
                audioSource.PlayOneShot(badHitClip, 0.2f);
                InitiateHapticFeedback(badHapticClip, 1);
                takeDamage(maxDamage / 4);
                break;
        }
    }

    public override void counter()
    {
        return;
    }

    public override void die() {
        GetComponent<Animator>().SetTrigger("kill");
        StopAllCoroutines();
        currentState = rangedState.dead;
        agent.enabled = false;
        spawnManager.EnemyKilled();
        GetComponent<Collider>().enabled = false;
        StartCoroutine("sink");
    }

    IEnumerator sink() {
        yield return new WaitForSeconds(5);
        for (int i = 0; i < 150; i++) {
            Vector3 newPosition = transform.position;
            newPosition.y -= 0.005F;
            transform.position = newPosition;
            yield return 0;
        }
        Destroy(gameObject);
    }
}
