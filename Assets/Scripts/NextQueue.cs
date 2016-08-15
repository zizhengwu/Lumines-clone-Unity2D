using System.Collections.Generic;
using UnityEngine;

public class NextQueue : MonoBehaviour {
    public GameObject group;
    private List<Transform> _groups;

    private static NextQueue _instance = null;

    public static NextQueue Instance {
        get {
            if (_instance == null) {
                _instance = GameObject.FindObjectOfType<NextQueue>();

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
    }

    // Update is called once per frame
    private void Update() {
    }

    public void GameOver() {
        _groups = null;
    }

    public Transform ReturnNext() {
        if (_groups == null) {
            _groups = new List<Transform>();
        }
        for (int i = _groups.Count; i < 3; i++) {
            GameObject g = Instantiate(group, transform.position + new Vector3(0, i * -2.5f), Quaternion.identity) as GameObject;
            _groups.Add(g.transform);
        }
        Transform nextGroup = _groups[0];
        _groups.RemoveAt(0);
        foreach (Transform g in _groups) {
            g.transform.position = g.transform.position + new Vector3(0, 2.5f);
        }
        GameObject generateNewGroup = Instantiate(group, transform.position + new Vector3(0, 2 * -2.5f), Quaternion.identity) as GameObject;
        _groups.Add(generateNewGroup.transform);
        return nextGroup;
    }
}