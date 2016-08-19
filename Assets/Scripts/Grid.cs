using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Grid : MonoBehaviour {

    // Grid size
    public static int Width = 16;

    public static int Height = 12;
    public GameObject InsideClearanceAnimationBlock;
    public GameObject Erased;
    public GameObject Clear;
    public static Transform[,] grid = new Transform[Width, Height];
    public static bool[,] ShouldClear = new bool[Width, Height];
    private static List<IntVector2> coordinatesToBeCleared = new List<IntVector2>();

    public static Transform CurrentGroup;

    private static Grid _instance = null;

    public static Grid Instance {
        get {
            if (_instance == null) {
                _instance = GameObject.FindObjectOfType<Grid>();

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
            if (grid[coordinate.x, coordinate.y].gameObject.GetComponent<Block>().Status == Block.State.ToBeErasedWhileFallingDown || grid[coordinate.x, coordinate.y].gameObject.GetComponent<Block>().Status == Block.State.ToBeErased) {
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
            if (grid[column, i] && grid[column, i].gameObject.GetComponent<Block>().Status == Block.State.ToBeErased) {
                Instantiate(InsideClearanceAnimationBlock, new Vector3(column + 0.5f, i + 0.5f), Quaternion.identity);
                grid[column, i].gameObject.GetComponent<Block>().Status = Block.State.InsideCurrentStreak;
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
                        grid[potentialColumn, h].gameObject.GetComponent<Block>().IsSameType(grid[column, h].gameObject.GetComponent<Block>()) &&
                        grid[potentialColumn, h + 1].gameObject.GetComponent<Block>().IsSameType(grid[column, h].gameObject.GetComponent<Block>())) {
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
                grid[column, h].gameObject.GetComponent<Block>().Status = Block.State.ToBeErased;
            }
            else {
                grid[column, h].gameObject.GetComponent<Block>().Status = Block.State.Normal;
            }
        }
    }

    private void CreateClearAtColumn(int column, bool[] toOrNotToBeErased) {
        int left = column - 1;
        int right = column + 1;
        for (int h = 0; h < Height; h++) {
            if (grid[column, h] != null && toOrNotToBeErased[h]) {
                if (h - 1 >= 0 && toOrNotToBeErased[h - 1] && grid[column, h].gameObject.GetComponent<Block>().Status == Block.State.Normal) {
                    // should be a clearance animation, left or right to be determined
                    if (left >= 0 && grid[left, h] && grid[left, h - 1] &&
                        grid[left, h].gameObject.GetComponent<Block>().IsSameType(grid[column, h].gameObject.GetComponent<Block>()) &&
                        grid[left, h - 1].gameObject.GetComponent<Block>().IsSameType(grid[column, h].gameObject.GetComponent<Block>())) {
                        Instantiate(Erased, new Vector3(column, h, -1), Quaternion.identity);
                    }
                    if (right < Width && grid[right, h] && grid[right, h - 1] &&
                        grid[right, h].gameObject.GetComponent<Block>().IsSameType(grid[column, h].gameObject.GetComponent<Block>()) &&
                        grid[right, h - 1].gameObject.GetComponent<Block>().IsSameType(grid[column, h].gameObject.GetComponent<Block>())) {
                        Instantiate(Erased, new Vector3(column + 1, h, -1), Quaternion.identity);
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
        GameObject parent = grid[column, height].parent.gameObject;
        parent.GetComponent<Group>().blocksRemaining -= 1;
        Destroy(grid[column, height].gameObject);
        if (parent.GetComponent<Group>().blocksRemaining == 0) {
            Destroy(parent);
        }
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
            if (grid[column, h].gameObject.GetComponent<Block>().Status == Block.State.InsideCurrentStreak) {
                exist = true;
                currentColumn.Add(new IntVector2(column, h));
            }
        }
        if (!exist || column == 0) {
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
                    Instantiate(Clear, new Vector3(coord.x, coord.y, -1), Quaternion.identity);
                }
            }
            JudgeAllColumns();
            coordinatesToBeCleared = new List<IntVector2>();
        }
        coordinatesToBeCleared.AddRange(currentColumn);
    }

    private void FallDownAtColumn(int column) {
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

    public int[] ColumnFullUntilHeight() {
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
}