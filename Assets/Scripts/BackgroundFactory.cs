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

using System;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.Networking;

public class BackgroundFactory : NetworkBehaviour {
    private GameObject _currentThemeGameObject;
    public GameObject[] BackgroundGameObjects;

    #region Singleton
    private static BackgroundFactory _instance = null;
    public static BackgroundFactory Instance {
        get {
            if (_instance == null) {
                _instance = GameObject.FindObjectOfType<BackgroundFactory>();
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
    public void HandleThemeChanged(object sender, EventArgs args) {
        if (_currentThemeGameObject != null) {
            Destroy(_currentThemeGameObject);
            NetworkServer.Destroy(_currentThemeGameObject);
        }
        
        GameObject blkgrd = BackgroundGameObjects[Random.Range(0, BackgroundGameObjects.Length)];
        _currentThemeGameObject = (GameObject)Instantiate(blkgrd, transform.localPosition, Quaternion.identity);
        NetworkServer.Spawn(_currentThemeGameObject);
    }
}