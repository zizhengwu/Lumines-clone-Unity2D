using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class LuminesNetworkManager : NetworkBehaviour {
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

    public void StartSingleMode() {
        playerNumber = 1;
        FindObjectOfType<NetworkManager>().StartHost();
    }

    public void StartCoopServer() {
        playerNumber = 2;
        FindObjectOfType<NetworkManager>().StartHost();
    }

    public void StartCoopClient() {
        FindObjectOfType<NetworkManager>().StartClient();
    }
}
