using UnityEngine;
using System.Collections;
using UnityEditor;

public class ThemeManager : MonoBehaviour {

    public Theme[] Themes;
    public static Theme CurrentTheme;

    // Use this for initialization
    void Awake() {
        CurrentTheme = Themes[Random.Range(0, Themes.Length)];
    }

    // Update is called once per frame
    void Update() {

    }
}
