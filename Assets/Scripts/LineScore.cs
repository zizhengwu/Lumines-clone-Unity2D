using System;
using UnityEngine;

public class LineScore : MonoBehaviour {

    // Use this for initialization
    private void Start() {
    }

    // Update is called once per frame
    private void Update() {
    }

    public void AddScore(int score) {
        GetComponent<TextMesh>().text = (Int32.Parse(GetComponent<TextMesh>().text) + score).ToString();
    }

    public void ToZero() {
        GetComponent<TextMesh>().text = "0";
    }
}