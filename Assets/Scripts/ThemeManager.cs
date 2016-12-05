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
using UnityEngine.Networking;

public class ThemeManager : NetworkBehaviour {
    public String[] ThemeNames;
    private List<Theme> Themes;

    public Theme CurrentTheme {
        get {
			return Themes[GameStatusSyncer.Instance.ThemeIndex];
        }
    }
    public string CurrentThemeName {
        get {
			return ThemeNames[GameStatusSyncer.Instance.ThemeIndex];
        }
        set {
            for (int i = 0; i < ThemeNames.Length; i++) {
                if (ThemeNames[i] == value) {
					GameStatusSyncer.Instance.ThemeIndex = i;
                    break;
                }
            }
        }
        
    }
	#region Sigleton
	private static ThemeManager _instance;
	public static ThemeManager Instance {
		get {
			if (_instance == null) {
				_instance = GameObject.FindObjectOfType<ThemeManager>();
			}
			return _instance;
		}
	}

	private void Awake() {
		if (_instance == null) {
			_instance = this;
		}
		else if (_instance != this) {
			Destroy(this.gameObject);
		}
	}
	#endregion

	[Command]
	public void CmdChangeTheme(){
		int rnd = Random.Range(0, Themes.Count);
		while (rnd == GameStatusSyncer.Instance.ThemeIndex) {
			rnd = Random.Range (0, Themes.Count);
		}

		GameStatusSyncer.Instance.ThemeIndex = rnd;
        SoundManager.Instance.RpcHandleThemeChange();
        BackgroundFactory.Instance.HandleThemeChanged();
        //ThemeLine.Instance.BeginThemeChange();
    }

    private void Start() {
        Themes = new List<Theme>();
        foreach (String themeName in ThemeNames) {
            Themes.Add(new Theme(themeName));
        }

		if (isServer) {
			CmdChangeTheme ();
		}
    }
}