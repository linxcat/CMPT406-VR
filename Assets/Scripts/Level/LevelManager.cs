using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {

    public int numberOfEnemies;
    public string nextScene;

    public GameObject fader;
    public Material red;
    public Material white;
    private bool isGameOver;
    private bool menuShown;
    private bool goNextScene;
    private float bufferTime = 5F;
    private int killedEnemies;
    public GameObject winningMenu;
    public GameObject gameoverMenu;

	// Use this for initialization
	void Start () {
        isGameOver = false;
        menuShown = false;
        goNextScene = false;
        killedEnemies = 0;
        //fader = GameObject.Find("Fader");
	}
	
	// Update is called once per frame
	void Update () {
        if (!menuShown && !isGameOver && killedEnemies >= numberOfEnemies) {
            isGameOver = true;
            menuShown = true;
            StartCoroutine ("showWinningMenu");
        } else if (!menuShown && isGameOver && killedEnemies < numberOfEnemies) {
            menuShown = true;
            StartCoroutine ("showGameoverMenu");
        }

        if (menuShown && OVRInput.Get (OVRInput.Button.One))
            goNextScene = true;
	}

    public void gameOver(){
        isGameOver = true;
    }

    public void enemyKilled(){
        killedEnemies++;
    }

    IEnumerator showWinningMenu(){
        winningMenu.SetActive (true);
        fader.GetComponent<Renderer>().material = white;
        fader.SetActive(true);
        yield return new WaitForSeconds(bufferTime);
        Time.timeScale = 0;
        while (!goNextScene)
            yield return null;
        Time.timeScale = 1;
        SceneManager.LoadScene(nextScene, LoadSceneMode.Single);
    }

    IEnumerator showGameoverMenu(){
        gameoverMenu.SetActive(true);
        fader.GetComponent<Renderer>().material = red;
        fader.SetActive(true);
        yield return new WaitForSeconds(bufferTime);
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
        return isGameOver;
    }
}
