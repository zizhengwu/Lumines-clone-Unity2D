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
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Gameover : MonoBehaviour {
    public Text Score;
    public GameObject AchiveHighscore;

    // Use this for initialization
    private void Start() {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    private void Update() {
    }

    public void ToggleEndMenu(int score) {
        SoundManager.Instance.StopTheme();
        gameObject.SetActive(true);
        Score.text = string.Format("Score          {0}", score.ToString());
        int oldHighscore = PlayerPrefs.GetInt("highscore", 0);
        AchiveHighscore.SetActive(false);
        if (score > oldHighscore) {
            AchiveHighscore.SetActive(true);
            PlayerPrefs.SetInt("highscore", score);
        }
    }

    public void BackToStartScreen() {
        GameManager.Instance.Mode = GameManager.GameModes.Menu;
        ThemeManager.Instance.CurrentThemeName = "Menu";
        NextQueue.Instance.GameOver();
        InputManager.Instance.GameOver();
        Grid.Instance.GameOver();
        SceneManager.LoadScene("start");
        Time.timeScale = 1;
    }
}