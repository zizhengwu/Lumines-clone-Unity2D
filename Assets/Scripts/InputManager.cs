using UnityEngine;

public class InputManager : MonoBehaviour {
    private static InputManager _instance = null;

    private int _moveFingerId = -1;
    private int _downFingerId = -1;
    private bool _moveFingerReady = true;
    private bool _downFingerReady = true;
    private Transform _preGroup = null;
    public float ScreenWidth;
    public float ScreenHeight;
    public float FallThreshold = 0.7f;
    private float _clockAndAntiBoundary;
    private float _upDownBoundary;
    private float _lastFall;

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

    public void GameOver() {
        _moveFingerId = -1;
        _downFingerId = -1;
        _moveFingerReady = true;
        _downFingerReady = true;
        _preGroup = null;
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
        updateBoundaray();
        _lastFall = GameManager.GameTime;
    }

    private void updateBoundaray() {
        ScreenWidth = Screen.width;
        ScreenHeight = Screen.height;
        Vector3 rightUp = ScreenToGridPoint(new Vector3(ScreenWidth, ScreenHeight, -10));
        _clockAndAntiBoundary = (rightUp.x + 16.5f) / 2;
        _upDownBoundary = rightUp.y / 2;
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
        if (GameManager.GameTime - _lastFall > FallThreshold) {
            Grid.CurrentGroup.GetComponent<Group>().MoveDown();
            updateLastFall();
        }
    }

    private void updateLastFall() {
        _lastFall = GameManager.GameTime;
    }

    private void HandleTouch(int touchFingerId, Vector3 touchPosition, TouchPhase touchPhase) {
        Vector2 position = ScreenToGridPoint(touchPosition);
        var x = position.x;
        var y = position.y;
        if (_preGroup == null) {
            _preGroup = Grid.CurrentGroup;
        }
        if (_preGroup != null && Grid.CurrentGroup != _preGroup) {
            if (_moveFingerId != -1) {
                _moveFingerReady = false;
            }
            if (_downFingerId != -1) {
                _downFingerReady = false;
            }
            _preGroup = Grid.CurrentGroup;
            return;
        }
        // downFinger is pressing
        if (touchFingerId == _downFingerId) {
            if (touchPhase == TouchPhase.Ended) {
                _downFingerId = -1;
                _downFingerReady = true;
            }
            else if (_downFingerReady) {
                Grid.CurrentGroup.GetComponent<Group>().MoveDown();
                updateLastFall();
            }
            return;
        }
        // begin
        if (touchPhase == TouchPhase.Began) {
            if (x > 17) {
                if (y < _upDownBoundary) {
                    _downFingerId = touchFingerId;
                    Grid.CurrentGroup.GetComponent<Group>().MoveDown();
                    updateLastFall();
                }
                else {
                    if (x < _clockAndAntiBoundary) {
                        Grid.CurrentGroup.GetComponent<Group>().AnticlockwiseRotate();
                    }
                    else {
                        Grid.CurrentGroup.GetComponent<Group>().ClockwiseRotate();
                    }
                }
            }
            else if (x < 16 && x >= -1) {
                _moveFingerId = touchFingerId;
            }
        }
        // move only applies to moveFinger
        else if (touchPhase == TouchPhase.Moved && touchFingerId == _moveFingerId && _moveFingerReady) {
            if (Mathf.Round(position.x) < Grid.CurrentGroup.transform.position.x) {
                Grid.CurrentGroup.GetComponent<Group>().MoveLeft();
            }
            else if (Mathf.Round(position.x) > Grid.CurrentGroup.transform.position.x) {
                Grid.CurrentGroup.GetComponent<Group>().MoveRight();
            }
        }
        // end only applies to moveFinger
        else if (touchPhase == TouchPhase.Ended) {
            if (touchFingerId == _moveFingerId) {
                _moveFingerId = -1;
                _moveFingerReady = true;
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