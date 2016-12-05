/*
 *  Lumines clone in Unity
 *
 *  Copyright (C) 2016 Zizheng Wu <me@zizhengwu.com>
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using UnityEngine;
using UnityEngine.Networking;

public class InputManager : NetworkBehaviour {
    /*
    private int _moveFingerId = -1;
    private int _downFingerId = -1;
    private bool _moveFingerReady = true;
    private bool _downFingerReady = true;
    private Transform _preGroup = null;
    */
    private float _clockAndAntiBoundary;
    private float _upDownBoundary;

    private float ScreenWidth;
    private float ScreenHeight;
    public float FallThreshold = 0.7f;
    private float _lastFall = 0f;

    // Use this for initialization
    public override void OnStartLocalPlayer() {    
        updateBoundaray();
        CmdToServer("register");
    }

    public void GameStart() {
        Debug.Log("GameStart");
        _lastFall = GameStatusSyncer.Instance.GameTime;
    }

    public void GameOver() {
        /*
        _moveFingerId = -1;
        _downFingerId = -1;
        _moveFingerReady = true;
        _downFingerReady = true;
        _preGroup = null;
        */
    }
    
    private void updateBoundaray() {
        ScreenWidth = Screen.width;
        ScreenHeight = Screen.height;
        Vector3 rightUp = ScreenToGridPoint(new Vector3(ScreenWidth, ScreenHeight, -10));
        _clockAndAntiBoundary = (rightUp.x + 16.5f) / 2;
        _upDownBoundary = rightUp.y / 2;
    }

    [Command]
    private void CmdToServer(string command) {
        switch (command) {
            case "register":
                GameManager.Instance.gameControl(this, GameManager.Command.init);
                break;
            case "moveDown":
                GameManager.Instance.groupControl(this, GameManager.InputCommand.moveDown);
                break;
            case "moveLeft":
                GameManager.Instance.groupControl(this, GameManager.InputCommand.moveLeft);
                break;
            case "moveRight":
                GameManager.Instance.groupControl(this, GameManager.InputCommand.moveRight);
                break;
            case "clockwiseRotate":
                GameManager.Instance.groupControl(this, GameManager.InputCommand.clockwiseRotate);
                break;
            case "anticlockwiseRotate":
                GameManager.Instance.groupControl(this, GameManager.InputCommand.anticlockwiseRotate);
                break;
			case "moveToButtom":
				GameManager.Instance.groupControl (this, GameManager.InputCommand.moveToButtom);
				break;
            default:
                Debug.Log("Unrecognized command: " + command);
                break;
        }
    }

    // Update is called once per frame
    private void Update() {
        // control group here
        if (!isLocalPlayer)
            return;

        if (GameStatusSyncer.Instance.isStart == false) {
            return;
        }

        /*
        #region Touch Support
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
        #endregion
        */
        #region KeyBoard Support
        if (Input.GetKeyDown(KeyCode.A)) {
            CmdToServer("moveLeft");
        }
        if (Input.GetKeyDown(KeyCode.D)) {
            CmdToServer("moveRight");
        }
        if (Input.GetKeyDown(KeyCode.S)) {
            CmdToServer("moveDown");
        }
        if (Input.GetKeyDown(KeyCode.K)) {
            CmdToServer("clockwiseRotate");
        }
        if (Input.GetKeyDown(KeyCode.J)) {
            CmdToServer("anticlockwiseRotate");
        }
		if (Input.GetKeyDown(KeyCode.L)) {
			CmdToServer("moveToButtom");
		}
        if (GameStatusSyncer.Instance.GameTime - _lastFall > FallThreshold) {
            CmdToServer("moveDown");
            _lastFall = GameStatusSyncer.Instance.GameTime;
        }
        #endregion
    }
    /*
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
                CmdToServer("moveDown");
                CmdToServer("updateLastFall");
            }
            return;
        }
        // begin
        if (touchPhase == TouchPhase.Began) {
            if (x > 17) {
                if (y < _upDownBoundary) {
                    _downFingerId = touchFingerId;
                    CmdToServer("moveDown");
                    CmdToServer("updateLastFall");
                }
                else {
                    if (x < _clockAndAntiBoundary) {
                        CmdToServer("anticlockwiseRotate");
                    }
                    else {
                        CmdToServer("clockwiseRotate");
                    }
                }
            }
            else if (x < 16 && x >= -1) {
                _moveFingerId = touchFingerId;
            }
        }
        // move only applies to moveFinger
        else if (touchPhase != TouchPhase.Ended && touchFingerId == _moveFingerId && _moveFingerReady) {
            if (Mathf.Round(position.x) < Grid.CurrentGroup.transform.position.x) {
                CmdToServer("moveLeft");
            }
            else if (Mathf.Round(position.x) > Grid.CurrentGroup.transform.position.x) {
                CmdToServer("moveRight");
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
    */
    public Vector3 ScreenToGridPoint(Vector3 position) {
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(position);
        return NormalizeCameraPoint(worldPoint);
    }

    private Vector3 NormalizeCameraPoint(Vector3 position) {
        return 8f / 5f * position + new Vector3(8, 7, 10);
    }
}