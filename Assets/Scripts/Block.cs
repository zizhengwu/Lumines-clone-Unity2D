using UnityEngine;
using System.Collections;

public class Block : MonoBehaviour {

    public enum State {
        Normal,
        ToBeErased,
        ToBeErasedWhileFallingDown,
        InsideCurrentStreak
    };

    public bool GoDown = false;
    public int DownTarget;
    public int Type = -1;
    private State status = State.Normal;

    public State Status {
        get { return status; }
        set {
            if (value != status) {
                if (value == State.ToBeErased) {
                    if (status == State.InsideCurrentStreak) {
                        return;
                    }
                    if (GoDown) {
                        status = State.ToBeErasedWhileFallingDown;
                    }
                    else {
                        status = State.ToBeErased;
                        ChangeSprite(State.ToBeErased);
                    }
                }
                else if (value == State.Normal) {
                    status = State.Normal;
                    ChangeSprite(State.Normal);
                }
                else if (value == State.InsideCurrentStreak) {
                    status = State.InsideCurrentStreak;
                    ChangeSprite(State.InsideCurrentStreak);
                }
            }
        }
    }

    private void ChangeSprite(State value) {
        if (value == State.ToBeErased) {
            switch (Type) {
                case 0:
                    gameObject.GetComponent<SpriteRenderer>().sprite = ThemeManager.CurrentTheme.Block0ToBeErased;
                    break;
                case 1:
                    gameObject.GetComponent<SpriteRenderer>().sprite = ThemeManager.CurrentTheme.Block1ToBeErased;
                    break;
            }
        }
        else if (value == State.InsideCurrentStreak) {
            gameObject.GetComponent<SpriteRenderer>().sprite = ThemeManager.CurrentTheme.InsideClearance;
        }
        else if (value == State.Normal) {
            switch (Type) {
                case 0:
                    gameObject.GetComponent<SpriteRenderer>().sprite = ThemeManager.CurrentTheme.Block0;
                    gameObject.GetComponent<Block>().Type = 0;
                    break;
                case 1:
                    gameObject.GetComponent<SpriteRenderer>().sprite = ThemeManager.CurrentTheme.Block1;
                    gameObject.GetComponent<Block>().Type = 1;
                    break;
            }
        }
    }

    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if (GoDown) {
	        DownToDepth();
	    }
	}

    public bool IsSameType(Block other) {
        return Type == other.Type;
    }

    private void DownToDepth() {
        Vector3 position = transform.position;
        Vector2 roundedPosition = Grid.RoundVector2(transform.position);
        if (position.y - 0.5 > DownTarget) {
            transform.position = new Vector3(position.x, position.y - 0.1f, position.z);
        }
        else {
            transform.position = new Vector3(position.x, roundedPosition.y + 0.5f, position.z);
            GoDown = false;
            if (status == State.ToBeErasedWhileFallingDown) {
                status = State.ToBeErased;
                ChangeSprite(State.ToBeErased);
            }
        }
    }
}
