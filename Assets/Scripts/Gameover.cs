using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Gameover : MonoBehaviour {
    public Text Score;

    // Use this for initialization
    private void Start() {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    private void Update() {
    }

    public void ToggleEndMenu(int score) {
        gameObject.SetActive(true);
        Score.text = score.ToString();
    }

    public void BackToStartScreen() {
        GameManager.Instance.Mode = GameManager.GameModes.Menu;
        ThemeManager.Instance.CurrentThemeName = "Menu";
        NextQueue.Instance.GameOver();
        InputManager.Instance.GameOver();
        Grid.Instance.GameOver();
        SceneManager.LoadScene("start");
        Time.timeScale = 1;
    }
}