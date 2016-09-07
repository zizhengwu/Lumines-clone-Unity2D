using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    private static GameManager _instance = null;

    public enum GameModes {
        Menu,
        Voyage
    }

    public GameModes Mode = GameModes.Menu;
    public float LastThemeSelected;

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

    private void Awake() {
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

    public static float GameTime;

    // Use this for initialization
    private void Start() {
    }

    public void Voyage() {
        Mode = GameModes.Voyage;
        LastThemeSelected = GameTime;
        ThemeManager.Instance.RandomTheme();
        SceneManager.LoadScene("game");
    }

    public void GameOver() {
        Debug.Log("gameover");
        Mode = GameModes.Menu;
        ThemeManager.Instance.CurrentThemeName = "Menu";
        NextQueue.Instance.GameOver();
        InputManager.Instance.GameOver();
        Grid.Instance.GameOver();
        SceneManager.LoadScene("start");
    }

    // Update is called once per frame
    private void Update() {
        GameTime = Time.time;
        if (Mode == GameModes.Voyage && GameTime - LastThemeSelected >= 15) {
            LastThemeSelected = GameTime;
            ThemeManager.Instance.RandomTheme();
            ChangeThemeDuringVoyage();
        }
    }

    public void ChangeThemeDuringVoyage() {
        GameObject themeLine = GameObject.Find("theme-line");
        themeLine.GetComponent<ThemeLine>().BeginThemeChange();
    }
}