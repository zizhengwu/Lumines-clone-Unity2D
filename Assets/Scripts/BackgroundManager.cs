using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class BackgroundManager : MonoBehaviour {
    private static BackgroundManager _instance = null;
    private GameObject _currentThemeGameObject;

    public GameObject[] BackgroundGameObjects;

    public static BackgroundManager Instance {
        get {
            if (_instance == null) {
                _instance = GameObject.FindObjectOfType<BackgroundManager>();

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

    // Use this for initialization
    private void Start() {
        _currentThemeGameObject = (GameObject)Instantiate(BackgroundGameObjects[Random.Range(0, BackgroundGameObjects.Length)], transform.position,
             Quaternion.identity);
        DontDestroyOnLoad(_currentThemeGameObject);
    }

    // Update is called once per frame
    private void Update() {
    }

    public void HandleThemeChanged(object sender, EventArgs args) {
        Destroy(_currentThemeGameObject);
        _currentThemeGameObject = (GameObject)Instantiate(BackgroundGameObjects[Random.Range(0, BackgroundGameObjects.Length)], transform.position,
             Quaternion.identity);
        DontDestroyOnLoad(_currentThemeGameObject);
    }
}