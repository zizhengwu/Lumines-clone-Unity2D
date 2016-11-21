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
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

// Server Only Object
public class Grid : NetworkBehaviour {
    // Grid size
    public const int Width = 16;
    public const int Height = 12;

    public GameObject InsideClearanceAnimationBlockPrefab;
    public GameObject ErasedPrefab;
    public GameObject ClearPrefab;
    public static Transform[,] grid = new Transform[Width, Height];
    public static bool[,] ShouldClear = new bool[Width, Height];
    private static List<IntVector2> coordinatesToBeCleared = new List<IntVector2>();


    #region Singleton
    private static Grid _instance = null;
    public static Grid Instance {
        get {
            if (_instance == null) {
                _instance = GameObject.FindObjectOfType<Grid>();
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
    
    public void GameOver() {
        grid = new Transform[Width, Height];
        ShouldClear = new bool[Width, Height];
        coordinatesToBeCleared = new List<IntVector2>();
    }

    // Update is called once per frame
    private void Update() {
    }

    public bool BlocksInsideClearance(List<IntVector2> coordinates) {
        foreach (IntVector2 coordinate in coordinates) {
            if (grid[coordinate.x, coordinate.y].GetComponent<Block>().Status == Block.State.ToBeErasedWhileFallingDown || grid[coordinate.x, coordinate.y].GetComponent<Block>().Status == Block.State.ToBeErased) {
                return true;
            }
        }
        return false;
    }

    public void JudgeInsideClearanceAtColumn(int column) {
        for (int i = 0; i < Height; i++) {
            if (grid[column, i] == null) {
                return;
            }
            if (grid[column, i] && grid[column, i].GetComponent<Block>().Status == Block.State.ToBeErased) {
                GameObject InsideClearanceAnimationBlock = (GameObject) Instantiate(InsideClearanceAnimationBlockPrefab, new Vector3(column + 0.5f, i + 0.5f), Quaternion.identity);
                NetworkServer.Spawn(InsideClearanceAnimationBlock);

                grid[column, i].GetComponent<Block>().Status = Block.State.InsideCurrentStreak;
                ShouldClear[column, i] = true;
            }
        }
    }

    public void JudgeClearAtColumn(int column) {
        if (column < 0 || column >= Width) {
            return;
        }
        bool[] toOrNotToBeErased = Enumerable.Repeat(false, Height).ToArray();

        for (int h = 0; h < Height; h++) {
            if (grid[column, h + 1] == null) {
                break;
            }
            if (grid[column, h].GetComponent<Block>().IsSameType(grid[column, h + 1].GetComponent<Block>())) {
                int columnLeft = column - 1;
                int columnRight = column + 1;
                List<int> potentialColumns = new List<int>();
                if (columnLeft >= 0) {
                    potentialColumns.Add(columnLeft);
                }
                if (columnRight < Width) {
                    potentialColumns.Add(columnRight);
                }

                foreach (int potentialColumn in potentialColumns) {
                    if (grid[potentialColumn, h] && grid[potentialColumn, h + 1] &&
                        grid[potentialColumn, h].GetComponent<Block>().IsSameType(grid[column, h].GetComponent<Block>()) &&
                        grid[potentialColumn, h + 1].GetComponent<Block>().IsSameType(grid[column, h].GetComponent<Block>())) {
                        toOrNotToBeErased[h] = true;
                        toOrNotToBeErased[h + 1] = true;
                        break;
                    }
                }
            }
        }

        UpdateClearAtColumn(column, toOrNotToBeErased);
    }

    private void UpdateClearAtColumn(int column, bool[] toOrNotToBeErased) {
        CreateClearAtColumn(column, toOrNotToBeErased);
        for (int h = 0; h < Height; h++) {
            if (grid[column, h] == null) {
                continue;
            }
            if (toOrNotToBeErased[h]) {
                grid[column, h].GetComponent<Block>().Status = Block.State.ToBeErased;
            }
            else {
                grid[column, h].GetComponent<Block>().Status = Block.State.Normal;
            }
        }
    }

    public void ChangeSpriteOnThemeChangeAtColumn(int column) {
        for (int h = 0; h < Height; h++) {
            if (grid[column, h] == null) {
                continue;
            }
            grid[column, h].GetComponent<Block>().SpriteThemeChange();
        }
    }

    private void CreateClearAtColumn(int column, bool[] toOrNotToBeErased) {
        int left = column - 1;
        int right = column + 1;
        for (int h = 0; h < Height; h++) {
            if (grid[column, h] != null && toOrNotToBeErased[h]) {
                if (h - 1 >= 0 && toOrNotToBeErased[h - 1] && grid[column, h].GetComponent<Block>().Status == Block.State.Normal) {
                    // should be a clearance animation, left or right to be determined
                    if (left >= 0 && grid[left, h] && grid[left, h - 1] &&
                        grid[left, h].GetComponent<Block>().IsSameType(grid[column, h].GetComponent<Block>()) &&
                        grid[left, h - 1].GetComponent<Block>().IsSameType(grid[column, h].GetComponent<Block>())) {
                        GameObject Erased = (GameObject) Instantiate(ErasedPrefab, new Vector3(column, h, -1), Quaternion.identity);
                        NetworkServer.Spawn(Erased);
                    }
                    if (right < Width && grid[right, h] && grid[right, h - 1] &&
                        grid[right, h].GetComponent<Block>().IsSameType(grid[column, h].GetComponent<Block>()) &&
                        grid[right, h - 1].GetComponent<Block>().IsSameType(grid[column, h].GetComponent<Block>())) {
                        GameObject Erased = (GameObject) Instantiate(ErasedPrefab, new Vector3(column + 1, h, -1), Quaternion.identity);
                        NetworkServer.Spawn(Erased);
                    }
                }
            }
        }
    }

    public void JudgeAllColumns() {
        for (int i = 0; i < Width; i++) {
            JudgeClearAtColumn(i);
        }
    }

    public void ClearAll() {
        for (int i = 0; i < Width; i++) {
            for (int j = 0; j < Height; j++) {
                if (ShouldClear[i, j]) {
                    destroyBlockAtGrid(i, j);
                }
            }
        }
        for (int i = 0; i < Width; i++) {
            FallDownAtColumn(i);
        }
    }

    private void destroyBlockAtGrid(int column, int height) {
        Group grp = grid[column, height].parent.GetComponent<Group>();

        grp.destroy(grid[column, height].GetComponent<Block>());
        grid[column, height] = null;
        ShouldClear[column, height] = false;
    }

    public void ClearBeforeColumn(int column) {
        bool exist = false;
        List<IntVector2> currentColumn = new List<IntVector2>();
        for (int h = 0; h < Height; h++) {
            if (!ValidCoordinate(column, h)) {
                break;
            }
            if (grid[column, h].GetComponent<Block>().Status == Block.State.InsideCurrentStreak) {
                exist = true;
                currentColumn.Add(new IntVector2(column, h));
            }
        }
        if (!exist || column == 0) {
            if (coordinatesToBeCleared.Count > 0) {
                var non = PrepareNonClearable();
                UpdateScore();
                DoClear();
                AfterClear(non);
            }
        }
        if (column == 0) {
            ScoreManager.Instance.ToZero();
        }
        coordinatesToBeCleared.AddRange(currentColumn);
    }

    private List<Transform> PrepareNonClearable() {
        var non = new List<Transform>();
        for (int i = 0; i < Width; i++) {
            for (int j = 0; j < Height; j++) {
                if (grid[i, j] != null && grid[i, j].GetComponent<Block>().Status == Block.State.Normal) {
                    non.Add(grid[i, j]);
                }
            }
        }
        return non;
    }

    private void AfterClear(List<Transform> non) {
        List<IntVector2> directions = new List<IntVector2> { new IntVector2(-1, 0), new IntVector2(1, 0), new IntVector2(0, -1), new IntVector2(0, 1) };
        List<IntVector2> visited = new List<IntVector2>();

        while (non.Count > 0) {
            var item = non.PopAt(0);
            var p = FindBlockPositionInGrid(item);
            if (grid[p.x, p.y].GetComponent<Block>().Status == Block.State.Normal) {
                continue;
            }
            SoundManager.Instance.CmdPlaySound(SoundManager.Sound.Clear);
            Stack<IntVector2> s = new Stack<IntVector2>();
            s.Push(p);
            while (s.Count > 0) {
                var i = s.Pop();
                visited.Add(i);
                non.Remove(grid[i.x, i.y]);
                foreach (var direction in directions) {
                    var neighbor = new IntVector2(i.x + direction.x, i.y + direction.y);
                    if (!visited.Contains(neighbor) && ValidCoordinate(neighbor) && grid[neighbor.x, neighbor.y].GetComponent<Block>().Status != Block.State.Normal) {
                        s.Push(neighbor);
                    }
                }
            }
        }
    }

    public IntVector2 FindBlockPositionInGrid(Transform t) {
        for (int i = 0; i < Width; i++) {
            for (int j = 0; j < Height; j++) {
                if (grid[i, j] != null && grid[i, j] == t) {
                    return new IntVector2(i, j);
                }
            }
        }

        return new IntVector2(-1, -1);
    }

    private void DoClear() {
        HashSet<int> columnsToFallDown = new HashSet<int>();
        foreach (IntVector2 coordinate in coordinatesToBeCleared) {
            destroyBlockAtGrid(coordinate.x, coordinate.y);
            columnsToFallDown.Add(coordinate.x);
        }
        foreach (int i in columnsToFallDown) {
            FallDownAtColumn(i);
        }
        foreach (IntVector2 coord in coordinatesToBeCleared) {
            if (coordinatesToBeCleared.Contains(new IntVector2(coord.x, coord.y - 1))) {
                GameObject Clear = (GameObject) Instantiate(ClearPrefab, new Vector3(coord.x, coord.y, -1), Quaternion.identity);
                NetworkServer.Spawn(Clear);
            }
        }
        JudgeAllColumns();
        coordinatesToBeCleared = new List<IntVector2>();
    }

    private void FallDownAtColumn(int column) {
        int current = 0;
        for (int h = 0; h < Height; h++) {
            if (grid[column, h] != null) {
                if (h == current) {
                }
                else {
                    grid[column, current] = grid[column, h];
                    grid[column, h] = null;
                    grid[column, current].GetComponent<Block>().setDownTarget(current);
                }
                current += 1;
            }
            else {
            }
        }
    }

    public int[] ColumnFullUntilHeight() {
        int[] columnsHeight = new int[Width];
        for (int i = 0; i < Width; i++) {
            columnsHeight[i] = 0;
            for (int j = 0; j < Height; j++) {
                if (grid[i, j] == null || GameManager.Instance.isPlayerGroup(grid[i, j].parent))
                    break;

                columnsHeight[i] += 1;
            }
        }
        return columnsHeight;
    }

    public Vector2 RoundVector2(Vector2 v) {
        return new Vector2(Mathf.Round(v.x - 0.5f),
                           Mathf.Round(v.y - 0.5f));
    }

    public bool InsideBorder(Vector2 pos) {
        return ((int)pos.x >= 0 &&
                (int)pos.x < Width &&
                (int)pos.y >= 0);
    }

    public bool ValidCoordinate(int x, int y) {
        if (x < 0 || x >= Width || y < 0 || y >= Height || grid[x, y] == null) {
            return false;
        }
        return true;
    }

    public bool ValidCoordinate(IntVector2 i) {
        return ValidCoordinate(i.x, i.y);
    }

    private void UpdateScore() {
        int count = 0;
        foreach (IntVector2 c in coordinatesToBeCleared) {
            int x = c.x;
            int y = c.y;
            if (coordinatesToBeCleared.Contains(new IntVector2(x, y + 1)) && coordinatesToBeCleared.Contains(new IntVector2(x + 1, y)) && coordinatesToBeCleared.Contains(new IntVector2(x + 1, y + 1))) {
                count += 1;
            }
        }
        ScoreManager.Instance.AddScore(Math.Max(count, 1));
    }
}