using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipManager : MonoBehaviour {

    public GameObject wave1, wave2, wave3, wave4, wave5;
    private int currentWave;
    private float oriTimeScale, timeout, tooltipBuffer;
    private SpawnManager spawnManager;
    private bool isPaused;

    private CharacterStats characterStats;

	// Use this for initialization
	void Start () {
        isPaused = false;
        currentWave = 0;
        timeout = 30;
        tooltipBuffer = 1;
        spawnManager = FindObjectOfType<SpawnManager> ();
        characterStats = FindObjectOfType<CharacterStats>();
	}
	
	// Update is called once per frame
	void Update () {
        if (currentWave < spawnManager.CurrentWave ()) {
            currentWave++;
            oriTimeScale = Time.timeScale;
            StartCoroutine ("ShowTooltip");
        }
        if (isPaused) {
            Time.timeScale = 0;
            if (OVRInput.GetDown (OVRInput.Button.PrimaryIndexTrigger))
                StartCoroutine ("Continue");
            if (OVRInput.GetUp (OVRInput.Button.PrimaryIndexTrigger))
                StopCoroutine ("Continue");
        }
	}

    IEnumerator ShowTooltip(){
        yield return new WaitForSeconds(tooltipBuffer);
        Time.timeScale = 0;
        isPaused = true;
        switch (currentWave) {
        case 1:
            wave1.SetActive (true);
            yield return new WaitForSecondsRealtime (timeout);
            wave1.SetActive (false);
            break;
        case 2:
            wave2.SetActive (true);
            yield return new WaitForSecondsRealtime (timeout);
            wave2.SetActive (false);
            break;
        case 3:
            wave3.SetActive (true);
            yield return new WaitForSecondsRealtime (timeout);
            wave3.SetActive (false);
            break;
        case 4:
            wave4.SetActive (true);
            characterStats.maxMana();
            yield return new WaitForSecondsRealtime (timeout);
            wave4.SetActive (false);
            break;
        case 5:
            wave5.SetActive (true);
            yield return new WaitForSecondsRealtime (timeout);
            wave5.SetActive (false);
            break;
        default:
            break;
        }
        Time.timeScale = oriTimeScale;
        isPaused = false;
    }

    IEnumerator Continue(){
        yield return new WaitForSecondsRealtime (2);
        Time.timeScale = oriTimeScale;
        isPaused = false;
        StopCoroutine("ShowTooltip");
        wave1.SetActive (false);
        wave2.SetActive (false);
        wave3.SetActive (false);
        wave4.SetActive (false);
        wave5.SetActive (false);
    }
}
