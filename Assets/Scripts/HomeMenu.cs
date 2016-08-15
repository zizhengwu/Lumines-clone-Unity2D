using UnityEngine;

public class HomeMenu : MonoBehaviour {

    private void Update() {
        if (Input.anyKeyDown) {
            GameManager.Instance.Voyage();
        }
    }
}