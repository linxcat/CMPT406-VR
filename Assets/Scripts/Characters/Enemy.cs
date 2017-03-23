using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : MonoBehaviour {

    //protected Material colourMaterial;
    protected GameObject player;
    protected NavMeshAgent agent;
    public int hp = 100;
    public int maxDamage = 50;

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
