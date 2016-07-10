using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Grid : MonoBehaviour {
    // Grid size
    public static int Width = 16;
    public static int Height = 12;

    public static Transform[,] grid = new Transform[Width, Height];
    public static bool[,] ShouldClear = new bool[Width, Height];

    public static Transform CurrentGroup;

    // Use this for initialization
    void Start() {

    }

    public static void GameOver() {
        grid = new Transform[Width, Height];
        ShouldClear = new bool[Width, Height];
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.F1)) {
            ClearAll();
        }
    }

    public static void JudgeInsideClearanceAtColumn(int column) {
        for (int i = 0; i < Height; i++) {
            if (grid[column, i] == null) {
                continue;
            }
            if (grid[column, i] && grid[column, i].gameObject.GetComponent<Block>().Status == Block.State.ToBeErased) {
                grid[column, i].gameObject.GetComponent<Block>().Status = Block.State.InsideCurrentStreak;
                ShouldClear[column, i] = true;
            }
        }
    }

    public static void JudgeClearAtColumn(int column) {
        if (column < 0 || column >= Width) {
            return;
        }
        bool[] toOrNotToBeErased = Enumerable.Repeat(false, Height).ToArray();

        for (int h = 0; h < Height; h++) {
            if (grid[column, h + 1] == null) {
                break;
            }
            if (grid[column, h].gameObject.GetComponent<Block>().IsSameType(grid[column, h + 1].gameObject.GetComponent<Block>())) {
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
                        grid[potentialColumn, h].gameObject.GetComponent<Block>()
                            .IsSameType(grid[column, h].gameObject.GetComponent<Block>()) && grid[potentialColumn, h + 1].gameObject.GetComponent<Block>()
                            .IsSameType(grid[column, h].gameObject.GetComponent<Block>())) {
                        toOrNotToBeErased[h] = true;
                        toOrNotToBeErased[h + 1] = true;
                        break;
                    }
                }
            }
        }

        for (int i = 0; i < Height; i++) {
            if (grid[column, i] == null) {
                continue;
            }
            if (toOrNotToBeErased[i]) {
                grid[column, i].gameObject.GetComponent<Block>().Status = Block.State.ToBeErased;
            }
            else {
                grid[column, i].gameObject.GetComponent<Block>().Status = Block.State.Normal;
            }
        }
    }

    public static void JudgeAllColumns() {
        for (int i = 0; i < Width; i++) {
            JudgeClearAtColumn(i);
        }
    }

    public static void ClearAll() {
        for (int i = 0; i < Width; i++) {
            for (int j = 0; j < Height; j++) {
                if (ShouldClear[i, j]) {
                    Destroy(grid[i, j].gameObject);
                    grid[i, j] = null;
                    ShouldClear[i, j] = false;
                }
            }
        }
        for (int i = 0; i < Width; i++) {
            FallDownAtColumn(i);
        }
    }

    private static void FallDownAtColumn(int column) {
        int current = 0;
        for (int h = 0; h < Height; h++) {
            if (grid[column, h] != null) {
                if (h == current) {
                }
                else {
                    grid[column, h].gameObject.GetComponent<Block>().DownTarget = current;
                    grid[column, current] = grid[column, h];
                    grid[column, h] = null;
                    grid[column, current].gameObject.GetComponent<Block>().GoDown = true;
                }
                current += 1;
            }
            else {
                
            }
        }
    }

    public static int[] ColumnFullUntilHeight() {
        int[] columnsHeight = new int[Width];
        for (int i = 0; i < Width; i++) {
            columnsHeight[i] = 0;
            for (int j = 0; j < Height; j++) {
                if (grid[i, j] != null && grid[i, j].parent != CurrentGroup) {
                    columnsHeight[i] += 1;
                }
                else {
                    break;
                }
            }
        }
        return columnsHeight;
    }

    public static Vector2 RoundVector2(Vector2 v) {
        return new Vector2(Mathf.Round(v.x - 0.5f),
                           Mathf.Round(v.y - 0.5f));
    }

    public static bool InsideBorder(Vector2 pos) {
        return ((int)pos.x >= 0 &&
                (int)pos.x < Width &&
                (int)pos.y >= 0);
    }
}
