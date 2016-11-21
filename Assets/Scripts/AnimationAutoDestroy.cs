using UnityEngine;
using UnityEngine.Networking;

public class AnimationAutoDestroy : NetworkBehaviour {
    public float delay = 0f;

    // Use this for initialization
    private void Start() {
        Invoke("CmdDestroy", GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length + delay);
    }

    [Command]
    private void CmdDestroy() {
        Destroy(gameObject);
        NetworkServer.Destroy(gameObject);
    }
}