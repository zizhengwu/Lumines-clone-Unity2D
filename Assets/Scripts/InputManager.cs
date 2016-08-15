using UnityEngine;

public class InputManager : MonoBehaviour {
    private static InputManager _instance = null;

    public static InputManager Instance {
        get {
            if (_instance == null) {
                _instance = GameObject.FindObjectOfType<InputManager>();

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
        // control group here
        if (Grid.CurrentGroup == null) {
            return;
        }
        if (Input.GetKeyDown(KeyCode.A)) {
            Grid.CurrentGroup.GetComponent<Group>().MoveLeft();
        }
        if (Input.GetKeyDown(KeyCode.D)) {
            Grid.CurrentGroup.GetComponent<Group>().MoveRight();
        }
        if (Input.GetKeyDown(KeyCode.S)) {
            Grid.CurrentGroup.GetComponent<Group>().MoveDown();
        }
        if (Input.GetKeyDown(KeyCode.K)) {
            Grid.CurrentGroup.GetComponent<Group>().ClockwiseRotate();
        }
        if (Input.GetKeyDown(KeyCode.J)) {
            Grid.CurrentGroup.GetComponent<Group>().AnticlockwiseRotate();
        }
    }
}