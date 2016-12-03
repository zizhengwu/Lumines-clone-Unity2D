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
using UnityEngine.Networking;

public class ScoreManager : NetworkBehaviour {
    public TextMesh ScoreFlag;
    public Text Score;
    public Text CurrentGameTime;
    public Text Highscore;
    public Text Clear;

    [SyncVar]
    private int tempScore = 0;

    #region Singleton
    private static ScoreManager _instance;
    public static ScoreManager Instance {
        get {
            if (_instance == null) {
                _instance = FindObjectOfType<ScoreManager>();
            }
            return _instance;
        }
    }

    private void Awake() {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            Destroy(gameObject);
    }
    #endregion
    
    [Client]
    private void Update() {
        if (GameStatusSyncer.Instance.isStart == false)
            return;

        float elapseTime = GameStatusSyncer.Instance.GameTime - GameStatusSyncer.Instance.GameStartTime;
        
        ScoreFlag.text = tempScore.ToString();
        Score.text = GameStatusSyncer.Instance.GameScore.ToString();
        CurrentGameTime.text = string.Format("{0}:{1:00}", (int) elapseTime / 60, (int) elapseTime % 60);
        Highscore.text = GameStatusSyncer.Instance.GameHighScore.ToString();
        Clear.text = string.Format("{0} %", (GameStatusSyncer.Instance.GameScore % 50) * 2);
    }

    [Server]
    public void AddScore(int score) {
        tempScore += score;

        int originalScore = GameStatusSyncer.Instance.GameScore;
        int finalScore = GameStatusSyncer.Instance.GameScore = originalScore + score;
        if ((originalScore < 10 && finalScore >= 10) || ((originalScore / 50) != (finalScore / 50))) {
            ThemeManager.Instance.ChangeTheme();
        }
    }
    
    [Server]
    public void ToZero() {
        tempScore = 0;
    }
}