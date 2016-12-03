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

// Multiple instance object in Network
public class Block : MonoBehaviour {
    public enum State {
        Normal,
        ToBeErased,
        ToBeErasedWhileFallingDown,
        InsideCurrentStreak
    };

    // Used for setting up animator on Server
    private bool _goDown = false;
    private int _downTarget;

    public bool Enabled {
        get {
            return transform.parent.GetComponent<Group>().getBlockEnabled(this);
        }
    }
    public int Type {
        get {
            return transform.parent.GetComponent<Group>().getBlockType(this);
        }
    }
    public State Status {
        get {
            return transform.parent.GetComponent<Group>().getBlockStatus(this);
        }
        set {
            State current = transform.parent.GetComponent<Group>().getBlockStatus(this);
            if (value == current)
                return;


            Group grp = transform.parent.GetComponent<Group>();
            switch (value) {
                case State.Normal:
                case State.InsideCurrentStreak:
                    grp.setBlockStatus(this, value);
                    break;
                case State.ToBeErased:
                    if (current == State.InsideCurrentStreak)
                        return;

                    if(_goDown)
                        grp.setBlockStatus(this, State.ToBeErasedWhileFallingDown);
                    else
                        grp.setBlockStatus(this, value);
                    break;
            }
        }
    }
    
    public void init(int value) {
        ;
    }
    
    public void setDownTarget(int target) {
        _downTarget = target;
        _goDown = true;
    }
    
    private void ChangeSprite(State value) {
        if (value == State.ToBeErased) {
            switch (Type) {
                case 0:
                    GetComponent<SpriteRenderer>().sprite = ThemeManager.Instance.CurrentTheme.Block0ToBeErased;
                    break;

                case 1:
                    GetComponent<SpriteRenderer>().sprite = ThemeManager.Instance.CurrentTheme.Block1ToBeErased;
                    break;
            }
        }
        else if (value == State.InsideCurrentStreak) {
            GetComponent<SpriteRenderer>().sprite = ThemeManager.Instance.CurrentTheme.InsideClearance;
        }
        else if (value == State.Normal) {
            switch (Type) {
                case 0:
                    GetComponent<SpriteRenderer>().sprite = ThemeManager.Instance.CurrentTheme.Block0;
                    break;

                case 1:
                    GetComponent<SpriteRenderer>().sprite = ThemeManager.Instance.CurrentTheme.Block1;
                    break;
            }
        }
    }
    
    public void SpriteThemeChange() {
        if (Status != State.InsideCurrentStreak) {
            ChangeSprite(Status);
        }
        else {
            ;
        }
    }
    
    private void Update() {
        if (Enabled == false) {
            enabled = false;
            GetComponent<SpriteRenderer>().enabled = false;
            return;
        }
        else {
            SpriteThemeChange();
            if (_goDown) {
                Vector3 targetPosition = new Vector3(transform.position.x, _downTarget + 0.5f, transform.position.z);
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, 10*Time.deltaTime);
                if (transform.position == targetPosition) {
                    _goDown = false;
                    if (Status == State.ToBeErasedWhileFallingDown) {
                        Status = State.ToBeErased;
                    }
                }
            }
        }
    }
    
    public bool IsSameType(Block other) {
        return Type == other.Type;
    }
}