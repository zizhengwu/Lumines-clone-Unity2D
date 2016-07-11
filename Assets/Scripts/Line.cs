using UnityEngine;
using System.Collections;

public class Line : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    Vector3 previousPosition = transform.position;
        transform.position = new Vector3(0 + (16 - 0) * (GameManager.GameTime % 4) / 4, previousPosition.y, previousPosition.z);
        // x is from 0 to 15
	    int x = (int) transform.position.x;
	    if ((int) transform.position.x != (int) previousPosition.x) {
	        Grid.JudgeInsideClearanceAtColumn((int) x);
	        if (x != 0) {
	            Grid.ClearBeforeColumn((int) transform.position.x);
	        }
	        else {
                Grid.ClearBeforeColumn(16);
            }
	    }
	}

}
