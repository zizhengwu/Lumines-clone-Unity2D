using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static float GameTime;

	// Use this for initialization
	void Start () {
	
	}

    public static void GameOver() {
        Grid.GameOver();
        Debug.Log("GAMEOVER");
        SceneManager.LoadScene("game");
    }

	// Update is called once per frame
	void Update () {
	    GameTime = Time.time;
	}
}
