using System.Collections.Generic;
using UnityEngine;

public class Group : MonoBehaviour {
    private int[] types = { 0, 1 };
    public int blocksRemaining = 4;

    // Use this for initialization
    private void Start() {
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

    public void MoveLeft() {
        SoundManager.Instance.PlaySound(SoundManager.Sound.Left);

        if (GroupIsValidGridPosition(transform.position + new Vector3(-1, 0, 0))) {
            transform.position += new Vector3(-1, 0, 0);
        }
    }

    public void MoveRight() {
        SoundManager.Instance.PlaySound(SoundManager.Sound.Right);

        if (GroupIsValidGridPosition(transform.position + new Vector3(1, 0, 0))) {
            transform.position += new Vector3(1, 0, 0);
        }
    }

    public void MoveDown() {
        if (GroupIsValidGridPosition(transform.position + new Vector3(0, -1, 0))) {
            transform.position += new Vector3(0, -1, 0);
        }
        else {
            SoundManager.Instance.PlaySound(SoundManager.Sound.Hit);
            Grid.CurrentGroup = null;
            FindObjectOfType<Spawner>().spawnNext();

            int[] columnsHeight = Grid.Instance.ColumnFullUntilHeight();
            foreach (Transform child in transform) {
                Vector3 v = child.localPosition;
                Vector2 gridV = Grid.Instance.RoundVector2(child.position);
                int downwardsGridY;
                if (v.y == 0.5) {
                    downwardsGridY = columnsHeight[(int)gridV.x];
                }
                else if (v.y == 1.5) {
                    downwardsGridY = columnsHeight[(int)gridV.x] + 1;
                }
                else {
                    throw new System.Exception();
                }
                if (downwardsGridY >= 10) {
                    GameManager.Instance.GameOver();
                    return;
                }
                child.gameObject.GetComponent<Block>().DownTarget = downwardsGridY;
                Grid.grid[(int)gridV.x, downwardsGridY] = child;
                child.gameObject.GetComponent<Block>().GoDown = true;
            }

            Grid.Instance.JudgeClearAtColumn((int)transform.position.x - 1);
            Grid.Instance.JudgeClearAtColumn((int)transform.position.x);
            Grid.Instance.JudgeClearAtColumn((int)transform.position.x + 1);
            Grid.Instance.JudgeClearAtColumn((int)transform.position.x + 2);
        }
    }

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

    // Update is called once per frame
    private void Update() {
    }
}