using UnityEngine;
using System.Collections;

public class Grid : MonoBehaviour {
    // Grid size
    public static int Width = 16;
    public static int Height = 12;

    public static Transform[,] grid = new Transform[Width, Height];

    public static Transform CurrentGroup;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
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
