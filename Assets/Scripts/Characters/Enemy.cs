using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : MonoBehaviour {

    //protected Material colourMaterial;
    protected GameObject player;
    protected NavMeshAgent agent;
    protected int hp;

    public int perfectDamage = 180;
    public int goodDamage = 100;
    public int badDamage = 50;

    //fraction of current health
    public float burnDamage = 0.03f;
    //fraction of a second
    public float burnTickSpeed = 0.25f;
    private IEnumerator currentBurn = null;

    protected float turnSpeed = 3F;

    public AudioSource audioSource;
    public AudioClip badHitClip;
    public AudioClip goodHitClip;
    public AudioClip perfectHitClip;

    // Use this for initialization
    public void Start () {
        agent = GetComponent<NavMeshAgent> ();
        audioSource = GetComponent<AudioSource>();
        player = GameObject.Find("Hitbox");
	}

    public abstract void swingHit(Hit hit);

    public abstract void counter();

    public virtual void takeDamage(int damage) {
        hp -= damage;
        if (!isAlive()) die();
    }

    public void startBurning(float seconds) {
        takeBurnTick();
        StopCoroutine(currentBurn);
        currentBurn = burn(seconds);
        StartCoroutine(currentBurn);
    }

    private void takeBurnTick() {
        hp = Mathf.RoundToInt(hp * (1f - burnDamage));
    }

    private IEnumerator burn(float seconds) {
        while (seconds > 0.0001) {
            takeBurnTick();
            yield return new WaitForSeconds(burnTickSpeed);
            seconds -= burnTickSpeed;
        }
    }

    public abstract void die();

    protected void slowFacePlayer()
    {
        Vector3 lookPos = player.transform.position - transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * turnSpeed);
    }

    protected void facePlayer() {
        Vector3 target = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
        transform.LookAt(target);
    }

    public bool isAlive()
    {
        if (hp <= 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public int getHp()
    {
        return hp;
    }

    //Call to initiate haptic feedback on a controller depending on the channel perameter. (Left controller is 0, right is 1)
    public void InitiateHapticFeedback(OVRHapticsClip hapticsClip, int channel) {
        OVRHaptics.Channels[channel].Mix(hapticsClip);
    }
}
