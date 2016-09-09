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

public class GameManager : MonoBehaviour {
    private static GameManager _instance = null;

    public enum GameModes {
        Menu,
        Voyage
    }

    public GameModes Mode = GameModes.Menu;
    public float LastThemeSelected;

    public static GameManager Instance {
        get {
            if (_instance == null) {
                _instance = GameObject.FindObjectOfType<GameManager>();

                //Tell unity not to destroy this object when loading a new scene!
                DontDestroyOnLoad(_instance.gameObject);
            }

            return _instance;
        }
    }

    private void Awake() {
        //Check if instance already exists
        if (_instance == null)

            //if not, set instance to this
            _instance = this;

        //If instance already exists and it's not this:
        else if (_instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
    }

    public static float GameTime;

    // Use this for initialization
    private void Start() {
        Application.targetFrameRate = 60;
    }

    public void Voyage() {
        Mode = GameModes.Voyage;
        LastThemeSelected = GameTime;
        ThemeManager.Instance.RandomTheme();
        SceneManager.LoadScene("game");
    }

    public void GameOver() {
        Grid.CurrentGroup = null;
        GameObject.Find("current-streak-score").GetComponent<ScoreManager>().GameOver();
        Debug.Log("gameover");
    }

    // Update is called once per frame
    private void Update() {
        GameTime = Time.time;
        //if (Mode == GameModes.Voyage && GameTime - LastThemeSelected >= 15) {
        //    LastThemeSelected = GameTime;
        //    ThemeManager.Instance.RandomTheme();
        //    ChangeThemeDuringVoyage();
        //}
    }

    public void ChangeThemeDuringVoyage() {
        GameObject themeLine = GameObject.Find("theme-line");
        themeLine.GetComponent<ThemeLine>().BeginThemeChange();
    }
}