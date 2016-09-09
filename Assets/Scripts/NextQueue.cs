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

    public void ChangeSpriteOnThemeChange() {
        foreach (Transform group in _groups) {
            foreach (Transform block in group) {
                block.GetComponent<Block>().SpriteThemeChange();
            }
        }
    }
}