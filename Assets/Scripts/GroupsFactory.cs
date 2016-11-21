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

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


// Server Only Object, All member function should be called on Server
public class GroupsFactory : NetworkBehaviour {
    public GameObject groupPrefab;
    public Transform QueuePosition;

    private List<Group> _groups = new List<Group>();
    public int maxGroupNumber = 3;

    #region Sigleton
    private static GroupsFactory _instance = null;
    public static GroupsFactory Instance {
        get {
            if (_instance == null) {
                _instance = GameObject.FindObjectOfType<GroupsFactory>();
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

    [Server]
    public void GameOver() {
        foreach(Group grp in _groups) {
            Destroy(grp.gameObject);
            NetworkServer.Destroy(grp.gameObject);
        }
        _groups.Clear();
    }

    [Server]
    public Group getNext() {
        queueRefill();

        Group group = _groups.PopAt(0);
        foreach (Group g in _groups) {
            g.transform.position += new Vector3(0, 2.5f);
        }

        queueRefill();
        return group;   
    }

    [Server]
    private void queueRefill() {
        for (int i = _groups.Count; i < maxGroupNumber; i++) {
            GameObject g = Instantiate(groupPrefab, QueuePosition.position + new Vector3(0, i * -2.5f), Quaternion.identity) as GameObject;
            NetworkServer.Spawn(g);


            g.GetComponent<Group>().init();
            _groups.Add(g.GetComponent<Group>());
        }
    }

    [Server]
    public void ChangeSpriteOnThemeChange() {
        foreach (Group group in _groups) {
            foreach (Transform block in group.transform) {
                block.GetComponent<Block>().SpriteThemeChange();
            }
        }
    }
}