using UnityEngine;

public class InputManager : MonoBehaviour {
    private static InputManager _instance = null;

    private int moveFingerId = -1;
    private int downFingerId = -1;
    private bool moveFingerReady = true;
    private bool downFingerReady = true;
    private Transform PreGroup = null;
    public float screenWidth;
    public float screenHeight;
    private float clockAndAntiBoundary;
    private float upDownBoundary;

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
        screenWidth = Screen.width;
        screenHeight = Screen.height;
        Vector3 rightUp = ScreenToGridPoint(new Vector3(screenWidth, screenHeight, -10));
        clockAndAntiBoundary = (rightUp.x + 16.5f) / 2;
        upDownBoundary = rightUp.y / 2;
    }

    // Update is called once per frame
    private void Update() {
        // control group here
        if (Grid.CurrentGroup == null) {
            return;
        }
        // Handle native touch events
        foreach (Touch touch in Input.touches) {
            HandleTouch(touch.fingerId, touch.position, touch.phase);
        }
        // Simulate touch events from mouse events
        if (Input.touchCount == 0) {
            if (Input.GetMouseButtonDown(0)) {
                HandleTouch(10, Input.mousePosition, TouchPhase.Began);
            }
            if (Input.GetMouseButton(0)) {
                HandleTouch(10, Input.mousePosition, TouchPhase.Moved);
            }
            if (Input.GetMouseButtonUp(0)) {
                HandleTouch(10, Input.mousePosition, TouchPhase.Ended);
            }
        }
        // Keyboard
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

    private void HandleTouch(int touchFingerId, Vector3 touchPosition, TouchPhase touchPhase) {
        Vector2 position = ScreenToGridPoint(touchPosition);
        var x = position.x;
        var y = position.y;
        if (PreGroup == null) {
            PreGroup = Grid.CurrentGroup;
        }
        if (PreGroup != null && Grid.CurrentGroup != PreGroup) {
            if (moveFingerId != -1) {
                moveFingerReady = false;
            }
            if (downFingerId != -1) {
                downFingerReady = false;
            }
            PreGroup = Grid.CurrentGroup;
            return;
        }
        // downFinger is pressing
        if (touchFingerId == downFingerId) {
            if (touchPhase == TouchPhase.Ended) {
                downFingerId = -1;
                downFingerReady = true;
            }
            else if (downFingerReady) {
                Grid.CurrentGroup.GetComponent<Group>().MoveDown();
            }
            return;
        }
        // begin
        if (touchPhase == TouchPhase.Began) {
            if (x > 17) {
                if (y < upDownBoundary) {
                    downFingerId = touchFingerId;
                    Grid.CurrentGroup.GetComponent<Group>().MoveDown();
                }
                else {
                    if (x < clockAndAntiBoundary) {
                        Grid.CurrentGroup.GetComponent<Group>().AnticlockwiseRotate();
                    }
                    else {
                        Grid.CurrentGroup.GetComponent<Group>().ClockwiseRotate();
                    }
                }
            }
            else if (x < 16 && x >= -1) {
                moveFingerId = touchFingerId;
            }
        }
        // move only applies to moveFinger
        else if (touchPhase == TouchPhase.Moved && touchFingerId == moveFingerId && moveFingerReady) {
            if (Mathf.Round(position.x) < Grid.CurrentGroup.transform.position.x) {
                Grid.CurrentGroup.GetComponent<Group>().MoveLeft();
            }
            else if (Mathf.Round(position.x) > Grid.CurrentGroup.transform.position.x) {
                Grid.CurrentGroup.GetComponent<Group>().MoveRight();
            }
        }
        // end only applies to moveFinger
        else if (touchPhase == TouchPhase.Ended) {
            if (touchFingerId == moveFingerId) {
                moveFingerId = -1;
                moveFingerReady = true;
            }
        }
    }

    public Vector3 ScreenToGridPoint(Vector3 position) {
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(position);
        return NormalizeCameraPoint(worldPoint);
    }

    public Vector3 NormalizeCameraPoint(Vector3 position) {
        return 8f / 5f * position + new Vector3(8, 5, 10);
    }
}