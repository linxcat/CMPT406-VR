using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRanged : Enemy {

    public GameObject projectile;
    private float detectRange = 100; //tuning required
    private float backupRange = 10;
    private float atkRange = 20;
    private float atkDelay = 5;
    private float speed = 2.5F;
    private int attackDmg = 30;
    private int searchAngle = 80;

    bool attacking = false;
    bool parryable = false;

    Animator anim;

    enum rangedState {
        idle,
        follow,
        backup,
        attack,
        parried,
        dead
    };

    private rangedState currentState = rangedState.idle;

    // Use this for initialization
    void Start() {
        base.Start();
        //TODO find projectile object
        turnSpeed = 4;
        anim = GetComponent<Animator>();
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update() {
        if (currentState == rangedState.parried) return;
        searchPlayer();
        switch (currentState) {
            case rangedState.follow:
                moveTowardsPlayer();
                break;
            case rangedState.backup:
                backUp();
                break;
            case rangedState.attack:
                if (!attacking) StartCoroutine("fireProjectile");
                break;
        }
    }

    private void searchPlayer() {
        anim.SetBool("moving", false);
        currentState = rangedState.idle;

        float angle = Vector3.Angle(player.transform.position - transform.position, transform.forward);
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (angle < searchAngle && distance < detectRange) {
            if (distance > atkRange) currentState = rangedState.follow;
            else if (distance < backupRange) currentState = rangedState.backup;
            else currentState = rangedState.attack;
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
            move(true);
            fall();
        }
    }

    void move(bool forward) {
        float step = speed * Time.deltaTime;
        if (!forward) step = -step;
        Vector3 targetPos = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPos, step);
    }

    void fall() {
        RaycastHit hitPoint = new RaycastHit();
        Vector3 aboveGround = transform.position;
        aboveGround.y += 1F;
        Physics.Raycast(aboveGround, Vector3.down, out hitPoint, float.MaxValue, LayerMask.GetMask(new string[] { "Ground" }));
        transform.position = hitPoint.point;
    }

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
    }

    IEnumerator fireProjectile() {
        anim.SetTrigger("attack");
        attacking = true;
        parryable = true;
        Invoke("spawnProjectile", 0.4f);
        yield return new WaitForSeconds(atkDelay);

        attacking = false;
        parryable = false;
        currentState = rangedState.idle;

    }

    private void spawnProjectile() {
        GameObject x = (GameObject)Instantiate(projectile);
        x.transform.position = transform.position + new Vector3(0, 1f, 0);
        x.GetComponent<Projectile>().setTarget(player);
    }

    public override void swingHit(Hit hit) {
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

    public override void counter() {
        if (!parryable) return;
        anim.SetTrigger("counter");
        StopCoroutine("attack");
        attacking = false;
        parryable = false;
        currentState = rangedState.parried;
    }

    public override void die() {
        GetComponent<Animator>().SetTrigger("kill");
        StopAllCoroutines();
        currentState = rangedState.dead;
        GetComponent<Collider>().enabled = false;
    }
}
