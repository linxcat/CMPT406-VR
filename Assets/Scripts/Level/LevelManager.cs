using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {

    public string nextScene;

    Fader fader;
    private bool isGameOver, isGameWon;
    private bool menuShown;
    private bool goNextScene;
    private float bufferTime = 3F;
    public GameObject winningMenu;
    public GameObject gameoverMenu;

    public AudioSource levelMusic;
    public AudioSource deathMusic;
    public AudioSource victoryMusic;



    void Awake() {
        fader = FindObjectOfType<Fader>();
    }

	// Use this for initialization
	void Start () {
        if (levelMusic != null) levelMusic.Play();

        isGameOver = false;
        isGameWon = false;
        menuShown = false;
        goNextScene = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (!menuShown && isGameWon) {
            menuShown = true;
            StartCoroutine ("showWinningMenu");
        } else if (!menuShown && isGameOver) {
            menuShown = true;
            StartCoroutine ("showGameoverMenu");
        }

        if (menuShown && OVRInput.GetDown (OVRInput.Button.One))
            goNextScene = true;
	}

    public void gameOver(){
        levelMusic.Stop();
        deathMusic.Play();
        isGameOver = true;

    }

    public void gameWon(){
        levelMusic.Stop();
        victoryMusic.Play();
        isGameWon = true;
    }

    IEnumerator showWinningMenu(){
        winningMenu.SetActive (true);
        fader.white();
        Time.timeScale = 0;
        while (!goNextScene)
            yield return null;
        Time.timeScale = 1;
        SceneManager.LoadScene(nextScene, LoadSceneMode.Single);
    }

    IEnumerator showGameoverMenu(){
        gameoverMenu.SetActive(true);
        fader.red();
        Time.timeScale = 0;
        while (!goNextScene)
            yield return null;
        Time.timeScale = 1;
        //Scene nextScene = SceneManager.CreateScene (SceneManager.GetActiveScene().name);
        //SceneManager.SetActiveScene (nextScene);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }

    public bool IsGameOver()
    {
        return isGameOver||isGameWon;
    }
}
