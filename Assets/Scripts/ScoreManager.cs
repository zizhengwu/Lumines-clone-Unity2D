using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {
    public Text Score;
    public Text Time;
    public Text Clear;
    private float _gameStartTime;

    // Use this for initialization
    private void Start() {
        _gameStartTime = GameManager.GameTime;
    }

    // Update is called once per frame
    private void Update() {
        float gameOngoingTime = GameManager.GameTime - _gameStartTime;
        Time.text = string.Format("{0}:{1:00}", (int)gameOngoingTime / 60, (int)gameOngoingTime % 60);
    }

    public void AddScore(int score) {
        GetComponent<TextMesh>().text = (int.Parse(GetComponent<TextMesh>().text) + score).ToString();
        int preScore = int.Parse(Score.text);
        int afterScore = preScore + score;
        UpdateProgress(preScore, afterScore);
        Score.text = afterScore.ToString();
    }

    public void ToZero() {
        GetComponent<TextMesh>().text = "0";
    }

    private void UpdateProgress(int preScore, int afterScore) {
        Clear.text = string.Format("{0} %", (afterScore % 50) * 2);
        if ((preScore / 50) != (afterScore / 50)) {
            ThemeManager.Instance.RandomTheme();
            GameManager.Instance.ChangeThemeDuringVoyage();
        }
    }
}