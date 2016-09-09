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