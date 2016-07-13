using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    private static GameManager _instance = null;

    public static GameManager Instance {
        get {
            if (_instance == null) {
                _instance = GameObject.FindObjectOfType<GameManager>();

                //Tell unity not to destroy this object when loading a new scene!
                DontDestroyOnLoad(_instance.gameObject);
            }

            return _instance;
        }
    }

    void Awake() {
        //Check if instance already exists
        if (_instance == null)

            //if not, set instance to this
            _instance = this;

        //If instance already exists and it's not this:
        else if (_instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
    }

    public static float GameTime = Time.time;

	// Use this for initialization
	void Start () {
	
	}

    public void Voyage() {
        ThemeManager.Instance.RandomTheme();
        SceneManager.LoadScene("game");
    }

    public void GameOver() {
        Debug.Log("gameover");
        ThemeManager.Instance.CurrentThemeName = "Menu";
        Grid.GameOver();
        SceneManager.LoadScene("start");
    }

    // Update is called once per frame
    void Update () {
	    GameTime = Time.time;
	}
}
