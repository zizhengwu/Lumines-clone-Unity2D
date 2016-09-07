using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerControl : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            //	BackgroundScroll.current.go ();
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            //	BackgroundScroll.current.go ();
        }

        if (Input.GetKey(KeyCode.Space))
        {
            switch (SceneManager.GetActiveScene().buildIndex)
            {

                case 0: SceneManager.LoadScene(1); break;
                case 1: SceneManager.LoadScene(2); break;
                case 2: SceneManager.LoadScene(3); break;
                case 3: SceneManager.LoadScene(4); break;
                case 4: SceneManager.LoadScene(5); break;
                case 5: SceneManager.LoadScene(6); break;
                case 6: SceneManager.LoadScene(7); break;
                case 7: SceneManager.LoadScene(8); break;
                case 8: SceneManager.LoadScene(9); break;
                case 9: SceneManager.LoadScene(0); break;


            }
        }
    }
}