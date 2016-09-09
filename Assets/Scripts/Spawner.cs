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

public class Spawner : MonoBehaviour {

    // Use this for initialization
    private void Start() {
        spawnNext();
        InputManager.Instance.GameOver();
    }

    // Update is called once per frame
    private void Update() {
    }

    public void spawnNext() {
        Transform group = NextQueue.Instance.ReturnNext();
        group.transform.position = transform.position;
        group.GetComponent<Group>().MakeItCurrentGroup();
    }
}