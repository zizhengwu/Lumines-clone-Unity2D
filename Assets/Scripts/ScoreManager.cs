/*
 *  Lumines clone in Unity
 *
 *  Copyright (C) 2016 Zizheng Wu <me@zizhengwu.com>
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {
    public Text Score;
    public Text CurrentGameTime;
    public Text Clear;
    public Text Highscore;
    public Gameover Gameover;
    private float _gameStartTime;

    // Use this for initialization
    private void Start() {
        _gameStartTime = GameManager.GameTime;
        Highscore.text = PlayerPrefs.GetInt("highscore", 0).ToString();
    }

    // Update is called once per frame
    private void Update() {
        float gameOngoingTime = GameManager.GameTime - _gameStartTime;
        CurrentGameTime.text = string.Format("{0}:{1:00}", (int)gameOngoingTime / 60, (int)gameOngoingTime % 60);
    }

    public void AddScore(int score) {
        GetComponent<TextMesh>().text = (int.Parse(GetComponent<TextMesh>().text) + score).ToString();
        int preScore = int.Parse(Score.text);
        int afterScore = preScore + score;
        UpdateProgress(preScore, afterScore);
        if (afterScore > int.Parse(Highscore.text)) {
            Highscore.text = afterScore.ToString();
        }
        Score.text = afterScore.ToString();
    }

    public void ToZero() {
        GetComponent<TextMesh>().text = "0";
    }

    private void UpdateProgress(int preScore, int afterScore) {
        Clear.text = string.Format("{0} %", (afterScore % 50) * 2);
        if ((preScore < 10 && afterScore >= 10) || ((preScore / 50) != (afterScore / 50))) {
            ThemeManager.Instance.RandomTheme();
            GameManager.Instance.ChangeThemeDuringVoyage();
        }
    }

    public void GameOver() {
        Time.timeScale = 0;
        Gameover.ToggleEndMenu(int.Parse(Score.text));
    }
}