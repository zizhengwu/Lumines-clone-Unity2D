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
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour {
    #region Singleton
    private static GameManager _instance = null;
    public static GameManager Instance {
        get {
            if (_instance == null) {
                _instance = GameObject.FindObjectOfType<GameManager>();
            }
            return _instance;
        }
    }

    private void Awake() {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            Destroy(gameObject);
    }
    #endregion

    public enum InputCommand {
        moveDown,
        moveRight,
        moveLeft,
        anticlockwiseRotate,
        clockwiseRotate,
		moveToButtom
    }

    public enum Command {
        init,
        renewGroup
    }

    private int playerNumber = 0;
    private GameObject[] startPos = null;
    private Dictionary<InputManager, Group> groupMap = new Dictionary<InputManager, Group>();
    private Dictionary<InputManager, Transform> posMap = new Dictionary<InputManager, Transform>();

    [Server]
    private void Start() {
        //TODO all clients must set targetFrameRate
        //Application.targetFrameRate = 60;
    }

    // Use this for initialization
    [Server]
    public override void OnStartServer() {
        playerNumber = LuminesNetworkManager.Instance.playerNumber;

        if (playerNumber == 1) {
            startPos = GameObject.FindGameObjectsWithTag("SinglePlayerPos");
        }
        else if (playerNumber > 1) {
            startPos = GameObject.FindGameObjectsWithTag("MultiPlayerPos");
        }
        else {
            throw new System.Exception("Unknown playerNumber: " + playerNumber);
        }
    }

    [Server]
    public void GameStart() {
        //ThemeManager.Instance.ChangeTheme();
        foreach (InputManager player in groupMap.Keys) {
            player.GameStart();
        }
        GameStatusSyncer.Instance.isStart = true;
    }

    [Server]
    public void GameOver() {
        posMap.Clear();
        groupMap.Clear();
        GameStatusSyncer.Instance.isStart = false;
        Gameover.Instance.RpcToggleEndMenu();
    }

    [Server]
    public void gameControl (InputManager player, Command command) {
        if (player == null)
            return;
		
        switch (command) {
            case Command.init:
                groupMap[player] = GroupsFactory.Instance.getNext();
                posMap[player] = startPos[groupMap.Keys.Count - 1].transform;
                groupMap[player].transform.position = posMap[player].position;
                break;
            case Command.renewGroup:
                groupMap[player] = GroupsFactory.Instance.getNext();
                groupMap[player].transform.position = posMap[player].position;
                break;
            default:
                throw new System.Exception("Unknown Game Command: " + command);
        }
    }

    [Server]
    public void groupControl(InputManager player, InputCommand command) {
        if (!existPlayer(player))
            gameControl(player, Command.renewGroup);

        switch (command) {
            case InputCommand.moveDown:
                groupMap[player].MoveDown();
                break;
            case InputCommand.moveLeft:
                groupMap[player].MoveLeft();
                break;
            case InputCommand.moveRight:
                groupMap[player].MoveRight();
                break;
            case InputCommand.anticlockwiseRotate:
                groupMap[player].AnticlockwiseRotate();
                break;
            case InputCommand.clockwiseRotate:
                groupMap[player].ClockwiseRotate();
                break;
			case InputCommand.moveToButtom:
				groupMap [player].MovetoButtom ();
				break;
            default:
                throw new System.Exception("Unknown Input Command: " + command);
        }        
    }

    [Server]
    public InputManager getPlayer(Group grp) {
        foreach (InputManager player in groupMap.Keys) {
            if (grp == groupMap[player])
                return player;
        }
        return null;
    }

    [Server]
    public bool existPlayer(InputManager player) {
        Group group = null;
        return groupMap.TryGetValue(player, out group);
    }

    [Server]
    public bool isPlayerGroup(Transform transform) {
        foreach (InputManager player in groupMap.Keys) {
            if (groupMap[player].transform == transform)
                return true;
        }
        return false;
    }
    
    [Server]
    private void Update() {
        if (!isServer)
            return;

        GameStatusSyncer.Instance.GameTime = Time.time;
        if (groupMap.Keys.Count == playerNumber && GameStatusSyncer.Instance.isStart == false) {
            GameStart();
        }
    }
}