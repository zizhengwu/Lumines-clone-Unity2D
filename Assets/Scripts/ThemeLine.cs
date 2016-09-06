using UnityEngine;

public class ThemeLine : MonoBehaviour {
    private bool _ongoing;
    private float _startTime;
    private bool _currentGroupChanged = false;

    // Use this for initialization
    private void Start() {
        _ongoing = false;
    }

    private void Update() {
        if (_ongoing) {
            Vector3 previousPosition = transform.position;
            transform.position = new Vector3(-5 + (22 - -5) * ((GameManager.GameTime - _startTime) % 3) / 3, previousPosition.y, previousPosition.z);
            // x is from -5 to 21
            for (int x = (int)transform.position.x; x > (int)previousPosition.x; x--) { 
                if (x == 21) {
                    _ongoing = false;
                    transform.position = new Vector3(-5, previousPosition.y, previousPosition.z);
                }
                if (0 <= x && x <= 15) {
                    Grid.Instance.ChangeSpriteOnThemeChangeAtColumn(x);
                }
                if (x == -4) {
                    NextQueue.Instance.ChangeSpriteOnThemeChange();
                }
                if (x >= (int)Grid.CurrentGroup.transform.position.x && !_currentGroupChanged) {
                    _currentGroupChanged = true;
                    foreach (Transform child in Grid.CurrentGroup) {
                        child.GetComponent<Block>().SpriteThemeChange();
                    }
                }
            }
        }
    }

    public void BeginThemeChange() {
        _startTime = GameManager.GameTime;
        _ongoing = true;
        _currentGroupChanged = false;
    }
}