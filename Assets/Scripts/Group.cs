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
    public int blocksRemaining = 4;

    // Use this for initialization
    void Start() {
        foreach (Transform child in transform) {
            GameObject c = child.gameObject;
            switch (types[Random.Range(0, 2)]) {
                case 0:
                    c.GetComponent<SpriteRenderer>().sprite = ThemeManager.Instance.CurrentTheme.Block0;
                    c.GetComponent<Block>().Type = 0;
                    break;
                case 1:
                    c.GetComponent<SpriteRenderer>().sprite = ThemeManager.Instance.CurrentTheme.Block1;
                    c.GetComponent<Block>().Type = 1;
                    break;
            }
        }
    }

    public void MakeItCurrentGroup() {
        Grid.CurrentGroup = transform;
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
        if (transform != Grid.CurrentGroup) {
            return;
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

        // Move Left
        else if (Input.GetKey(KeyCode.A)) {
            if ((consecutiveLeft && GameManager.GameTime - _lastLeft >= 0.03) || !consecutiveLeft) {
                SoundManager.Instance.PlaySound(SoundManager.Sound.Left);
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
            SoundManager.Instance.PlaySound(SoundManager.Sound.Right);
            if ((consecutiveRight && GameManager.GameTime - _lastRight >= 0.03) || !consecutiveRight) {
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
        if ((Input.GetKey(KeyCode.S) && GameManager.GameTime - _lastFall >= 0.03) || GameManager.GameTime - _lastFall >= 1) {
            _lastFall = GameManager.GameTime;
            List<IntVector2> blocksCoordinate = new List<IntVector2>();
            if (GroupIsValidGridPosition(transform.position + new Vector3(0, -1, 0))) {
                transform.position += new Vector3(0, -1, 0);
                //BlocksCoordinate.AddRange(new List<IntVector2>() {
                //    new IntVector2((int)Grid.RoundVector2(transform.position).x, (int)Grid.RoundVector2(transform.position).y),
                //    new IntVector2((int)Grid.RoundVector2(transform.position).x + 1, (int)Grid.RoundVector2(transform.position).y),
                //    new IntVector2((int)Grid.RoundVector2(transform.position).x, (int)Grid.RoundVector2(transform.position).y + 1),
                //    new IntVector2((int)Grid.RoundVector2(transform.position).x + 1, (int)Grid.RoundVector2(transform.position).y + 1)
                //});
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
                        enabled = false;
                        GameManager.Instance.GameOver();
                        return;
                    }
                    child.gameObject.GetComponent<Block>().DownTarget = downwardsGridY;
                    Grid.grid[(int) gridV.x, downwardsGridY] = child;
                    blocksCoordinate.Add(new IntVector2((int)gridV.x, downwardsGridY));
                    child.gameObject.GetComponent<Block>().GoDown = true;
                }

                Grid.JudgeClearAtColumn((int) transform.position.x - 1);
                Grid.JudgeClearAtColumn((int) transform.position.x);
                Grid.JudgeClearAtColumn((int) transform.position.x + 1);
                Grid.JudgeClearAtColumn((int) transform.position.x + 2);

            }
            if (blocksCoordinate.Count != 0) {
                if (Grid.BlocksInsideClearance(blocksCoordinate)) {
                    SoundManager.Instance.PlaySound(SoundManager.Sound.Clear);
                }
            }
        }
    }
}
