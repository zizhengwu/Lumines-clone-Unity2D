using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class ThemeManager : MonoBehaviour {
    public Theme[] Themes;
    public Theme CurrentTheme;
    private string _currentThemeName;

    public event EventHandler ThemeChanged;

    public string CurrentThemeName {
        set {
            _currentThemeName = value;
            EventHandler handler = ThemeChanged;
            handler(this, EventArgs.Empty);
        }
        get { return _currentThemeName; }
    }

    private static ThemeManager _instance;

    public static ThemeManager Instance {
        get {
            if (_instance == null) {
                _instance = GameObject.FindObjectOfType<ThemeManager>();

                //Tell unity not to destroy this object when loading a new scene!
                DontDestroyOnLoad(_instance.gameObject);
            }

            return _instance;
        }
    }

    // Use this for initialization
    private void Awake() {
        if (_instance == null) {
            //If I am the first instance, make me the Singleton
            _instance = this;
            ThemeChanged += SoundManager.Instance.HandleThemeChanged;
            DontDestroyOnLoad(this);
        }
        else {
            //If a Singleton already exists and you find
            //another reference in scene, destroy it!
            if (this != _instance)
                Destroy(this.gameObject);
        }
    }

    public void RandomTheme() {
        CurrentTheme = Themes[Random.Range(0, Themes.Length)];
        CurrentThemeName = CurrentTheme.ThemeName;
    }

    // Update is called once per frame
    private void Update() {
    }
}