using System;
using UnityEngine;
using UnityEngine.UI;

public class LineScore : MonoBehaviour {
    public Text Score;
    // Use this for initialization
    private void Start() {
    }

    // Update is called once per frame
    private void Update() {
    }

    public void AddScore(int score) {
        GetComponent<TextMesh>().text = (int.Parse(GetComponent<TextMesh>().text) + score).ToString();
        Score.text = (int.Parse(GetComponent<TextMesh>().text) + score).ToString();
    }

    public void ToZero() {
        GetComponent<TextMesh>().text = "0";
    }
}