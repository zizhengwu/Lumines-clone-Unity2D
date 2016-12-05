using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LuminesMenuController : MonoBehaviour {
    public InputField ServerIPInput;

    public void StartSingleMode() {
        Debug.Log("start single");
        LuminesNetworkManager.Instance.playerNumber = 1;
        FindObjectOfType<NetworkManager>().StartHost();
    }

    public void StartCoopServer() {
        Debug.Log("start coop server");
        LuminesNetworkManager.Instance.playerNumber = 2;
        FindObjectOfType<NetworkManager>().StartHost();
    }

    public void StartCoopClient() {
        Debug.Log("start coop server");
        FindObjectOfType<NetworkManager>().StartClient();
    }

    public void onServerIPEdited() {
        Debug.Log(ServerIPInput.text);
        FindObjectOfType<NetworkManager>().networkAddress = ServerIPInput.text;
    }
}
