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

// Multiple instance Object
public class MovingLine : NetworkBehaviour {
    public float speed = 0.25f;
    
    private Vector3 _target;
    private void Start() {
        _target = new Vector3(16, transform.position.y, transform.position.z);
    }

    private void FixedUpdate() {
        if (!isServer || GameStatusSyncer.Instance.isStart == false)  
            return;

        // 4s for a complete span, x in [0, 16)
        Vector3 nextPosition = Vector3.MoveTowards(transform.position, _target, Time.fixedDeltaTime/speed);
        if (nextPosition == _target) nextPosition.x = 0;
        
        if ((int) nextPosition.x != (int) transform.position.x) {
            int column = (int) nextPosition.x;
            Grid.Instance.JudgeInsideClearanceAtColumn(column);
            Grid.Instance.ClearBeforeColumn(column);
        }
        transform.position = nextPosition;
    }
}