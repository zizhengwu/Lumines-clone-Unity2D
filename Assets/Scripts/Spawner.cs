using UnityEngine;

public class Spawner : MonoBehaviour {

    // Use this for initialization
    private void Start() {
        spawnNext();
        InputManager.Instance.GameOver();
    }

    // Update is called once per frame
    private void Update() {
    }

    public void spawnNext() {
        Transform group = NextQueue.Instance.ReturnNext();
        group.transform.position = transform.position;
        group.GetComponent<Group>().MakeItCurrentGroup();
    }
}