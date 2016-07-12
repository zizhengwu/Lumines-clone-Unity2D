using UnityEngine;
using System.Collections;

public class ThemeManager : MonoBehaviour {

    public Theme[] Themes;
    public static Theme CurrentTheme;
    public static string CurrentThemeName;

    // Use this for initialization
    void Awake() {
        CurrentTheme = Themes[Random.Range(0, Themes.Length)];
        CurrentThemeName = CurrentTheme.ThemeName;
        SoundManager.Instance.changeTheme();
        SoundManager.Instance.PlaySound(SoundManager.Sound.Theme);
    }

    // Update is called once per frame
    void Update() {

    }
}
