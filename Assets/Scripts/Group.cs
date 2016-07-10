using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Group : MonoBehaviour {

    private int[] types = {0, 1};

    // Use this for initialization
    void Start() {
        Grid.CurrentGroup = transform;
        foreach (Transform child in transform) {
            GameObject c = child.gameObject;
            switch (types[Random.Range(0, 2)]) {
                case 0:
                    c.GetComponent<SpriteRenderer>().sprite = ThemeManager.CurrentTheme.Block1;
                    c.GetComponent<Block>().Type = 0;
                    break;
                case 1:
                    c.GetComponent<SpriteRenderer>().sprite = ThemeManager.CurrentTheme.Block2;
                    c.GetComponent<Block>().Type = 1;
                    break;
            }
            
            
        }
    }

    bool GroupIsValidGridPosition(Vector3 GroupVector) {
        Vector2[] children = {new Vector2(GroupVector.x + 0.5f, GroupVector.y + 0.5f), new Vector2(GroupVector.x + 0.5f, GroupVector.y + 1.5f), new Vector2(GroupVector.x + 1.5f, GroupVector.y + 1.5f), new Vector2(GroupVector.x + 1.5f, GroupVector.y + 0.5f)};
        foreach (Vector2 child in children) {
            Vector2 v = Grid.RoundVector2(child);

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

    // Update is called once per frame
    void Update() {
        // Move Left
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            if (GroupIsValidGridPosition(transform.position + new Vector3(-1, 0, 0))) {
                transform.position += new Vector3(-1, 0, 0);
            }
            else {

            }
        }

        // Move Right
        else if (Input.GetKeyDown(KeyCode.RightArrow)) {
            if (GroupIsValidGridPosition(transform.position + new Vector3(1, 0, 0))) {
                transform.position += new Vector3(1, 0, 0);
            }
            else {

            }
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
        }

        else if (Input.GetKeyDown(KeyCode.DownArrow)) {
            if (GroupIsValidGridPosition(transform.position + new Vector3(0, -1, 0))) {
                transform.position += new Vector3(0, -1, 0);
                // It's valid. Update grid.
            }
            else {
                enabled = false;
                FindObjectOfType<Spawner>().spawnNext();

                int[] columnsHeight = Grid.ColumnFullUntilHeight();
                foreach (Transform child in transform) {
                    Vector3 v = child.localPosition;
                    Vector2 gridV = Grid.RoundVector2(child.position);
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
                        Debug.Log("GAMEOVER");
                        SceneManager.LoadScene("game");
                    }
                    child.gameObject.GetComponent<Block>().DownTarget = downwardsGridY;
                    Grid.grid[(int) gridV.x, downwardsGridY] = child;
                    child.gameObject.GetComponent<Block>().GoDown = true;
                }

            }

        }
    }
}
