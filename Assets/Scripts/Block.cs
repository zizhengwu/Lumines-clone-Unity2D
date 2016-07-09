using UnityEngine;
using System.Collections;

public class Block : MonoBehaviour {

    public bool GoDown { get; set; }
    public int DownTarget { get; set; }
    public int Type;

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
        }
    }
}
