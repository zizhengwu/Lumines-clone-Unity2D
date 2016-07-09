using UnityEngine;
using System.Collections;

public class Group : MonoBehaviour {

    // Use this for initialization
    void Start() {

    }

    bool BlockIsValidGridPosition(Transform block) {
        return true;
    }

    bool GroupIsValidGridPosition() {
        foreach (Transform child in transform) {
            Vector2 v = Grid.RoundVector2(child.position);

            // Not inside Border?
            if (!Grid.InsideBorder(v))
                return false;

            // Block in grid cell (and not part of same group)?
            if (Grid.grid[(int)v.x, (int)v.y] != null &&
                Grid.grid[(int)v.x, (int)v.y].parent != transform)
                return false;
        }
        return true;
    }

    private void UpdateGrid() {
        // Remove old children from grid
        for (int y = 0; y < Grid.Height; ++y)
            for (int x = 0; x < Grid.Width; ++x)
                if (Grid.grid[x, y] != null)
                    if (Grid.grid[x, y].parent == transform)
                        Grid.grid[x, y] = null;

        // Add new children to grid
        foreach (Transform child in transform) {
            Vector2 v = Grid.RoundVector2(child.position);
            Grid.grid[(int)v.x, (int)v.y] = child;
        }
    }

    // Update is called once per frame
    void Update() {
        // Move Left
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            // Modify position
            transform.position += new Vector3(-1, 0, 0);

            // See if valid
            if (GroupIsValidGridPosition())
                // It's valid. Update grid.
                UpdateGrid();
            else
                // It's not valid. revert.
                transform.position += new Vector3(1, 0, 0);
        }

        // Move Right
        else if (Input.GetKeyDown(KeyCode.RightArrow)) {
            // Modify position
            transform.position += new Vector3(1, 0, 0);

            // See if valid
            if (GroupIsValidGridPosition())
                // It's valid. Update grid.
                UpdateGrid();
            else
                // It's not valid. revert.
                transform.position += new Vector3(-1, 0, 0);
        }

        // Rotate
        else if (Input.GetKeyDown(KeyCode.UpArrow)) {
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
            foreach (Transform child in transform) {
                Vector2 v = Grid.RoundVector2(child.position);
                Grid.grid[(int)v.x, (int)v.y] = child;
            }
        }

        else if (Input.GetKeyDown(KeyCode.DownArrow)) {
            
        }

    }
}
