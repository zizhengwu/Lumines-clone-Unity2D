using UnityEngine;

public class BackgroundScroll : MonoBehaviour {
    public float speed = 0;
    public BackgroundScroll current;
    public float tileSize = 0;
    private float pos = 0;
    private Renderer my_renderer;

    // Use this for initialization
    private void Start() {
        current = this;
    }

    // Update is called once per frame

    private void Update() {
        pos += speed;
        if (pos < -tileSize)
            pos += tileSize;

        current.transform.position = new Vector3(pos, current.transform.position.y, current.transform.position.z);
        //my_renderer.material.mainTextureOffset = new Vector2 (pos, 0);
    }
}