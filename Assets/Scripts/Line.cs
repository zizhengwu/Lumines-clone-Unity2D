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
	    if (transform.position.x < previousPosition.x) {
	        Grid.ClearAll();
            Grid.JudgeAllColumns();
	    }
	}

}
