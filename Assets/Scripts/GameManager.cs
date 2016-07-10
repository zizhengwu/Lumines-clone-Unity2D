using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
    public static float GameTime;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    GameTime = Time.time;
	}
}
