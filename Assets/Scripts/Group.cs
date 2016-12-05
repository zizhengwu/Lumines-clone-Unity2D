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

// Multiple instance object in Network
public class Group : NetworkBehaviour {
    public int blockRemaining = 4;
    private SyncListInt _blockType = new SyncListInt();
    private SyncListInt _blockStatus = new SyncListInt();
    private SyncListBool _blockEnabled = new SyncListBool();

    [Server]
    public void init() {
        // Initial status of each block
        foreach (Transform child in transform){
            int type = Random.Range(0, 2);

            _blockType.Add(type);
            _blockStatus.Add((int) Block.State.Normal);
            _blockEnabled.Add(true);
        }
    }

    [Server]
    public void destroy(Block blk) {
        int index = getBlockIndex(blk);

        _blockEnabled[index] = false;
        if (--blockRemaining == 0) {
            Destroy(gameObject);
            NetworkServer.Destroy(gameObject);
        }
    }

    [Server]
    public void setBlockStatus(Block blk, Block.State status) {
        int index = getBlockIndex(blk);
        _blockStatus[index] = (int) status;
    }

    [Client]
    public int getBlockType(Block blk) {
        return _blockType[getBlockIndex(blk)];
    }

    [Client]
    public Block.State getBlockStatus(Block blk) {
        return (Block.State) _blockStatus[getBlockIndex(blk)];
    }

    [Client]
    public bool getBlockEnabled(Block blk) {
        return _blockEnabled[getBlockIndex(blk)];
    }

    [Client]
    private int getBlockIndex(Block blk) {
        for (int i = 0; i < _blockType.Count; i++) {
            Transform child = transform.GetChild(i).transform;
            if (child == blk.transform)
                return i;
        }
        throw new System.Exception("cant find Block index: " + blk);
    }
    
    [Server]
    private bool GroupIsValidGridPosition(Vector3 GroupVector) {
        Vector2[] children = { new Vector2(GroupVector.x + 0.5f, GroupVector.y + 0.5f), new Vector2(GroupVector.x + 0.5f, GroupVector.y + 1.5f), new Vector2(GroupVector.x + 1.5f, GroupVector.y + 1.5f), new Vector2(GroupVector.x + 1.5f, GroupVector.y + 0.5f) };
        foreach (Vector2 child in children) {
            Vector2 v = Grid.Instance.RoundVector2(child);

            // Not inside Border?
            if (!Grid.Instance.InsideBorder(v))
                return false;

            // Block in grid cell (and not part of same group)?
            if (Grid.grid[(int)v.x, (int)v.y] != null &&
                Grid.grid[(int)v.x, (int)v.y].parent != transform)
                return false;
        }
        return true;
    }

    [Server]
    public void MoveLeft() {
        SoundManager.Instance.CmdPlaySound(SoundManager.Sound.Left);

        if (GroupIsValidGridPosition(transform.position + new Vector3(-1, 0, 0))) {
            transform.position += new Vector3(-1, 0, 0);
        }
    }

    [Server]
    public void MoveRight() {
        SoundManager.Instance.CmdPlaySound(SoundManager.Sound.Right);

        if (GroupIsValidGridPosition(transform.position + new Vector3(1, 0, 0))) {
            transform.position += new Vector3(1, 0, 0);
        }
    }

    [Server]
    public void MoveDown() {
        if (GroupIsValidGridPosition(transform.position + new Vector3(0, -1, 0))) {
            transform.position += new Vector3(0, -1, 0);
        }
        else {
            SoundManager.Instance.CmdPlaySound(SoundManager.Sound.Hit);
            GameManager.Instance.gameControl(GameManager.Instance.getPlayer(this), GameManager.Command.renewGroup);
            MovetoButtom();
        }
    }

    [Server]
    public void MovetoButtom() {
        List<IntVector2> blocksCoordinate = new List<IntVector2>();
        int[] columnsHeight = Grid.Instance.ColumnFullUntilHeight();
        foreach (Transform child in transform) {
            Vector3 v = child.localPosition;
            Vector2 gridV = Grid.Instance.RoundVector2(child.position);
            int downwardsGridY;
            if (v.y == 0.5) {
                downwardsGridY = columnsHeight[(int) gridV.x];
            }
            else if (v.y == 1.5) {
                downwardsGridY = columnsHeight[(int) gridV.x] + 1;
            }
            else {
                throw new System.Exception();
            }
            
            if (downwardsGridY >= 10) {
                GameManager.Instance.GameOver();
                return;
            }
            Grid.grid[(int) gridV.x, downwardsGridY] = child;
            blocksCoordinate.Add(new IntVector2((int) gridV.x, downwardsGridY));

            child.GetComponent<Block>().setDownTarget(downwardsGridY);
        }
        
        Grid.Instance.JudgeClearAtColumn((int) transform.position.x - 1);
        Grid.Instance.JudgeClearAtColumn((int) transform.position.x    );
        Grid.Instance.JudgeClearAtColumn((int) transform.position.x + 1);
        Grid.Instance.JudgeClearAtColumn((int) transform.position.x + 2);
        if (blocksCoordinate.Count != 0) {
            if (Grid.Instance.BlocksInsideClearance(blocksCoordinate)) {
                SoundManager.Instance.CmdPlaySound(SoundManager.Sound.Clear);
            }
        }
    }

    [Server]
    public void ClockwiseRotate() {
        foreach (Transform child in transform) {
            Vector3 v = child.localPosition;

            if (v.x == 0.5 && v.y == 1.5) {
                child.localPosition = new Vector3(1.5f, 1.5f, v.z);
            }
            else if (v.x == 1.5 && v.y == 1.5) {
                child.localPosition = new Vector3(1.5f, 0.5f, v.z);
            }
            else if (v.x == 1.5 && v.y == 0.5) {
                child.localPosition = new Vector3(0.5f, 0.5f, v.z);
            }
            else if (v.x == 0.5 && v.y == 0.5) {
                child.localPosition = new Vector3(0.5f, 1.5f, v.z);
            }
            else {
                throw new System.Exception();
            }
        }
    }

    [Server]
    public void AnticlockwiseRotate() {
        foreach (Transform child in transform) {
            Vector3 v = child.localPosition;

            if (v.x == 0.5 && v.y == 1.5) {
                child.localPosition = new Vector3(0.5f, 0.5f, v.z);
            }
            else if (v.x == 1.5 && v.y == 1.5) {
                child.localPosition = new Vector3(0.5f, 1.5f, v.z);
            }
            else if (v.x == 1.5 && v.y == 0.5) {
                child.localPosition = new Vector3(1.5f, 1.5f, v.z);
            }
            else if (v.x == 0.5 && v.y == 0.5) {
                child.localPosition = new Vector3(1.5f, 0.5f, v.z);
            }
            else {
                throw new System.Exception();
            }
        }
    }
}