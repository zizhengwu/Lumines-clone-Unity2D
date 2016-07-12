using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class HomeMenu : MonoBehaviour {

    void Update() {
        if (Input.anyKeyDown) {
            SceneManager.LoadScene("game");
        }
    }
}
