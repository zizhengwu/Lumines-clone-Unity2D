using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Group : MonoBehaviour {

    private int[] types = {0, 1};
    private static float _lastFall = GameManager.GameTime;
    private float _lastLeft = GameManager.GameTime;
    private float _lastRight = GameManager.GameTime;
    private bool consecutiveLeft = false;
    private bool consecutiveRight = false;

    // Use this for initialization
    void Start() {
        Grid.CurrentGroup = transform;
        foreach (Transform child in transform) {
            GameObject c = child.gameObject;
            switch (types[Random.Range(0, 2)]) {
                case 0:
                    c.GetComponent<SpriteRenderer>().sprite = ThemeManager.CurrentTheme.Block0;
                    c.GetComponent<Block>().Type = 0;
                    break;
                case 1:
                    c.GetComponent<SpriteRenderer>().sprite = ThemeManager.CurrentTheme.Block1;
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
        if (Input.GetKey(KeyCode.A)) {
            if ((consecutiveLeft && GameManager.GameTime - _lastLeft >= 0.07) || !consecutiveLeft) {
                _lastLeft = GameManager.GameTime;
                if (!consecutiveLeft) {
                    consecutiveLeft = true;
                    _lastLeft += 0.2f;
                }
                
                if (GroupIsValidGridPosition(transform.position + new Vector3(-1, 0, 0))) {
                    transform.position += new Vector3(-1, 0, 0);
                }
            }
        }
        else if (Input.GetKeyUp(KeyCode.A)) {
            consecutiveLeft = false;
        }

        // Move Right
        else if (Input.GetKey(KeyCode.D)) {
            if ((consecutiveRight && GameManager.GameTime - _lastRight >= 0.07) || !consecutiveRight) {
                _lastRight = GameManager.GameTime;
                if (!consecutiveRight) {
                    consecutiveRight = true;
                    _lastRight += 0.2f;
                }

                if (GroupIsValidGridPosition(transform.position + new Vector3(1, 0, 0))) {
                    transform.position += new Vector3(1, 0, 0);
                }
            }
        }
        else if (Input.GetKeyUp(KeyCode.D)) {
            consecutiveRight = false;
        }

        // Move Down
        if ((Input.GetKey(KeyCode.S) && GameManager.GameTime - _lastFall >= 0.07) || GameManager.GameTime - _lastFall >= 1) {
            _lastFall = GameManager.GameTime;
            if (GroupIsValidGridPosition(transform.position + new Vector3(0, -1, 0))) {
                transform.position += new Vector3(0, -1, 0);
                // It's valid. Update grid.
            }
            else {
                enabled = false;
                _lastFall += 0.2f;
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
                        GameManager.GameOver();
                    }
                    child.gameObject.GetComponent<Block>().DownTarget = downwardsGridY;
                    Grid.grid[(int) gridV.x, downwardsGridY] = child;
                    child.gameObject.GetComponent<Block>().GoDown = true;
                }

                Grid.JudgeClearAtColumn((int) transform.position.x - 1);
                Grid.JudgeClearAtColumn((int) transform.position.x);
                Grid.JudgeClearAtColumn((int) transform.position.x + 1);
                Grid.JudgeClearAtColumn((int) transform.position.x + 2);
            }

        }

        // Clockwise Rotate
        if (Input.GetKeyDown(KeyCode.K)) {
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
        // Anticlockwise Rotate
        else if (Input.GetKeyDown(KeyCode.J)) {
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
}
