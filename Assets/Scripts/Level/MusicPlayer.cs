using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour {

    public static AudioSource audioSource;
    public AudioClip levelMusic;
    public AudioClip deathMusic;
    public AudioClip winMusic;


	// Use this for initialization
	void Start () {
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void playDeath() {
        audioSource.Stop();
        audioSource.PlayOneShot(deathMusic);
    }

    public void playVictory(){
        audioSource.Stop();
        audioSource.PlayOneShot(winMusic);
    }
}
