using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class LuminesNetworkManager : MonoBehaviour {
    #region Singleton
    private static LuminesNetworkManager _instance;
    public static LuminesNetworkManager Instance {
        get {
            if (_instance == null) {
                _instance = FindObjectOfType<LuminesNetworkManager>();
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

    public int playerNumber = 1;
}
