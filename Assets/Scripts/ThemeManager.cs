/*
 *  Lumines clone in Unity
 *
 *  Copyright (C) 2016 Zizheng Wu <me@zizhengwu.com>
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ThemeManager : MonoBehaviour {
    public List<Theme> Themes;
    public Theme CurrentTheme;
    public String[] ThemeNames;
    private string _currentThemeName;

    public event EventHandler ThemeChanged;

    public string CurrentThemeName {
        set {
            _currentThemeName = value;
            EventHandler handler = ThemeChanged;
            handler(this, EventArgs.Empty);
        }
        get { return _currentThemeName; }
    }

    private static ThemeManager _instance;

    public static ThemeManager Instance {
        get {
            if (_instance == null) {
                _instance = GameObject.FindObjectOfType<ThemeManager>();

                //Tell unity not to destroy this object when loading a new scene!
                DontDestroyOnLoad(_instance.gameObject);
            }

            return _instance;
        }
    }

    // Use this for initialization
    private void Awake() {
        if (_instance == null) {
            //If I am the first instance, make me the Singleton
            _instance = this;
            ThemeChanged += SoundManager.Instance.HandleThemeChanged;
            ThemeChanged += BackgroundManager.Instance.HandleThemeChanged;
            DontDestroyOnLoad(this);
        }
        else {
            //If a Singleton already exists and you find
            //another reference in scene, destroy it!
            if (this != _instance)
                Destroy(this.gameObject);
        }
    }

    public void RandomTheme() {
        string nextThemeName = CurrentThemeName;
        int nextThemeNumber = -1;
        while (nextThemeName == CurrentThemeName) {
            nextThemeNumber = Random.Range(0, Themes.Count);
            nextThemeName = Themes[nextThemeNumber].ThemeName;
        }
        Debug.Log(nextThemeName);
        CurrentTheme = Themes[nextThemeNumber];
        CurrentThemeName = CurrentTheme.ThemeName;
    }

    // Update is called once per frame
    private void Update() {
    }

    public void ChangeTheme() {
        ThemeManager.Instance.RandomTheme();
        GameObject themeLine = GameObject.Find("theme-line");
        themeLine.GetComponent<ThemeLine>().BeginThemeChange();
    }

    private void Start() {
        Themes = new List<Theme>();
        foreach (String themeName in ThemeNames) {
            Themes.Add(new Theme(themeName));
        }
    }
}