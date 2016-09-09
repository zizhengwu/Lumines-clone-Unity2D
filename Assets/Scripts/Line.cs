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

public class Line : MonoBehaviour {

    // Use this for initialization
    private void Start() {
    }

    // Update is called once per frame
    private void Update() {
    }

    private void FixedUpdate() {
        Vector3 previousPosition = transform.position;
        transform.position = new Vector3(0 + (16 - 0) * (GameManager.GameTime % 4) / 4, previousPosition.y, previousPosition.z);
        // x is from 0 to 15
        int x = (int)transform.position.x;
        if ((int)transform.position.x != (int)previousPosition.x) {
            Grid.Instance.JudgeInsideClearanceAtColumn(x);
            Grid.Instance.ClearBeforeColumn(x);
        }
    }
}