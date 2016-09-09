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

public class ThemeLine : MonoBehaviour {
    private bool _ongoing;
    private float _startTime;
    private bool _currentGroupChanged = false;

    // Use this for initialization
    private void Start() {
        _ongoing = false;
        GetComponent<Renderer>().enabled = false;
    }

    private void Update() {
        if (_ongoing) {
            Vector3 previousPosition = transform.position;
            transform.position = new Vector3(-5 + (22 - -5) * ((GameManager.GameTime - _startTime) % 3) / 3, previousPosition.y, previousPosition.z);
            // x is from -5 to 21
            for (int x = (int)transform.position.x; x > (int)previousPosition.x; x--) {
                if (x >= 21) {
                    _ongoing = false;
                    GetComponent<Renderer>().enabled = false;
                    transform.position = new Vector3(-5, previousPosition.y, previousPosition.z);
                }
                if (0 <= x && x <= 15) {
                    Grid.Instance.ChangeSpriteOnThemeChangeAtColumn(x);
                }
                if (x == -4) {
                    NextQueue.Instance.ChangeSpriteOnThemeChange();
                }
                if (x >= (int)Grid.CurrentGroup.transform.position.x && !_currentGroupChanged) {
                    _currentGroupChanged = true;
                    foreach (Transform child in Grid.CurrentGroup) {
                        child.GetComponent<Block>().SpriteThemeChange();
                    }
                }
            }
        }
    }

    public void BeginThemeChange() {
        _startTime = GameManager.GameTime;
        _ongoing = true;
        GetComponent<Renderer>().enabled = true;
        _currentGroupChanged = false;
    }
}