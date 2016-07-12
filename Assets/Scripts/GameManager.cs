using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager Instance = null;

    void Awake() {
        //Check if instance already exists
        if (Instance == null)

            //if not, set instance to this
            Instance = this;

        //If instance already exists and it's not this:
        else if (Instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
    }

    public static float GameTime;

	// Use this for initialization
	void Start () {
	
	}

    public void GameOver() {
        Debug.Log("gameover");
        Grid.GameOver();
        SceneManager.LoadScene("start");
    }

    // Update is called once per frame
    void Update () {
	    GameTime = Time.time;
	}
}
