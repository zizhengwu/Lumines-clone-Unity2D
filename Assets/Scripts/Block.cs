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

public class Block : MonoBehaviour {

    public enum State {
        Normal,
        ToBeErased,
        ToBeErasedWhileFallingDown,
        InsideCurrentStreak
    };

    public bool GoDown = false;
    public int DownTarget;
    public int Type = -1;
    private State status = State.Normal;

    public State Status {
        get { return status; }
        set {
            if (value != status) {
                if (value == State.ToBeErased) {
                    if (status == State.InsideCurrentStreak) {
                        return;
                    }
                    if (GoDown) {
                        status = State.ToBeErasedWhileFallingDown;
                    }
                    else {
                        status = State.ToBeErased;
                        ChangeSprite(State.ToBeErased);
                    }
                }
                else if (value == State.Normal) {
                    status = State.Normal;
                    ChangeSprite(State.Normal);
                }
                else if (value == State.InsideCurrentStreak) {
                    status = State.InsideCurrentStreak;
                    ChangeSprite(State.InsideCurrentStreak);
                }
            }
        }
    }

    private void ChangeSprite(State value) {
        if (value == State.ToBeErased) {
            switch (Type) {
                case 0:
                    gameObject.GetComponent<SpriteRenderer>().sprite = ThemeManager.Instance.CurrentTheme.Block0ToBeErased;
                    break;

                case 1:
                    gameObject.GetComponent<SpriteRenderer>().sprite = ThemeManager.Instance.CurrentTheme.Block1ToBeErased;
                    break;
            }
        }
        else if (value == State.InsideCurrentStreak) {
            gameObject.GetComponent<SpriteRenderer>().sprite = ThemeManager.Instance.CurrentTheme.InsideClearance;
        }
        else if (value == State.Normal) {
            switch (Type) {
                case 0:
                    gameObject.GetComponent<SpriteRenderer>().sprite = ThemeManager.Instance.CurrentTheme.Block0;
                    break;

                case 1:
                    gameObject.GetComponent<SpriteRenderer>().sprite = ThemeManager.Instance.CurrentTheme.Block1;
                    break;
            }
        }
    }

    // Use this for initialization
    private void Start() {
    }

    public void SpriteThemeChange() {
        if (Status != State.InsideCurrentStreak) {
            ChangeSprite(Status);
        }
    }

    // Update is called once per frame
    private void Update() {
        if (GoDown) {
            DownToDepth();
        }
    }

    public bool IsSameType(Block other) {
        return Type == other.Type;
    }

    private void DownToDepth() {
        Vector3 position = transform.position;
        Vector2 roundedPosition = Grid.Instance.RoundVector2(transform.position);
        if (position.y - 0.5 > DownTarget) {
            transform.position = new Vector3(position.x, position.y - 0.4f, position.z);
        }
        else {
            transform.position = new Vector3(position.x, roundedPosition.y + 0.5f, position.z);
            GoDown = false;
            if (status == State.ToBeErasedWhileFallingDown) {
                status = State.ToBeErased;
                ChangeSprite(State.ToBeErased);
            }
        }
    }
}