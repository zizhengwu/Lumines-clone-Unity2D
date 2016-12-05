using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class GameStatusSyncer : NetworkBehaviour {
    #region Singleton
    private static GameStatusSyncer _instance;
    public static GameStatusSyncer Instance {
        get {
            if (_instance == null) {
                _instance = FindObjectOfType<GameStatusSyncer>();
            }
            return _instance;
        }
    }
    private void Awake() {
        if (_instance == null) {
            _instance = this;
        }
        else if (_instance != this) {
            Destroy(gameObject);
        }
    }
    #endregion

    [SyncVar]
    private bool _isStart = false;
    public bool isStart {
        get {
            return _isStart;
        }
        set {
            if (!isServer)
                return;
            _isStart = value;

            if (_isStart == true) {
                GameStartTime = GameTime;
            }
        }
    }

    [SyncVar]
    private float _gameTime = 0f;
    public float GameTime {
        get {
            return _gameTime;
        }
        set {
            if (!isServer)
                return;
            _gameTime = value;
        }
    }

    [SyncVar]
    private float _gameStartTime = 0f;
    public float GameStartTime {
        get {
            return _gameStartTime;
        }
        set {
            if (!isServer)
                return;
            _gameStartTime = GameTime;
        }
    }

    [SyncVar]
    private int _gameScore = 0;
    public int GameScore {
        get {
            return _gameScore;
        }
        set {
            if (!isServer)
                return;

            _gameScore = value;
            if (_gameScore > _gameHighScore)
                _gameHighScore = _gameScore;
        }
    }

    [SyncVar]
    private int _gameHighScore = 0;
    public int GameHighScore {
        get {
            return _gameHighScore;
        }
    }

	[SyncVar]
	private int _themeIndex = 0;
	public int ThemeIndex {
		get {
			return _themeIndex;
		}
		set {
			if (!isServer)
				return;

			_themeIndex = value;
		}
	}

    private void Start() {
        if (!isServer)
            return;

        _gameHighScore = PlayerPrefs.GetInt("highscore", 0);
    }
}
