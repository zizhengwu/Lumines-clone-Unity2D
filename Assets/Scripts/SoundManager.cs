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
using UnityEngine.Networking;

// Multiple instance objects
public class SoundManager : NetworkBehaviour {
    private AudioSource left;
    private AudioSource right;
    private AudioSource theme;
    private AudioSource clockwise;
    private AudioSource anticlockwise;
    private AudioSource hit;
    private List<AudioSource> clear;
    private List<AudioSource>.Enumerator clearIterator;
    private float lastClear = 0;

    public enum Sound {
        Left,
        Right,
        Theme,
        Clockwise,
        AntiClockwise,
        Hit,
        Clear
    }

    #region Singleton
    private static SoundManager _instance;
    public static SoundManager Instance {
        get {
            if (_instance == null) {
                _instance = GameObject.FindObjectOfType<SoundManager>();
            }

            return _instance;
        }
    }

    private void Awake() {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            Destroy(gameObject);
    }
    #endregion

    #region HandleThemeChange
	[ClientRpc]    
    public void RpcHandleThemeChange() {
        var rnd = new System.Random();
        string prefix = "Sounds/" + rnd.Next(1, 6) + "/";

        theme.volume = 0.5f;

        left.clip = Resources.Load(prefix + "left") as AudioClip;
        right.clip = Resources.Load(prefix + "right") as AudioClip;
        theme.clip = Resources.Load(prefix + "theme") as AudioClip;
        clockwise.clip = Resources.Load(prefix + "clockwise") as AudioClip;
        anticlockwise.clip = Resources.Load(prefix + "anticlockwise") as AudioClip;
        hit.clip = Resources.Load(prefix + "hit") as AudioClip;
        for (int i = 1; i <= 5; i++) {
            AudioClip clip = Resources.Load(prefix + i) as AudioClip;
            clear[i - 1].clip = clip;
        }
        GetNewClearIterator();

        if (!theme.isPlaying && ThemeManager.Instance.CurrentThemeName != "Menu") {
            PlaySound(Sound.Theme);
        }
    }
    #endregion

    #region StopTheme
    [Command] 
    public void CmdStopTheme() {
        if (!isServer)
            return;

        if (theme) {
            RpcStopTheme();
        }
    }
    
    [ClientRpc]
    private void RpcStopTheme() {
        if (theme) {
            theme.Stop();
        }
    }
    #endregion

    #region PlaySound
    [Command]
    public void CmdPlaySound(Sound sound) {
        if (!isServer)
            return;

        RpcPlaySound(sound);
    }

    [ClientRpc]
    private void RpcPlaySound(Sound sound) {
        PlaySound(sound);
    }

    [Client]
    private void PlaySound(Sound sound) {
        if (sound == Sound.Left) {
            left.Play();
        }
        if (sound == Sound.Right) {
            right.Play();
        }
        if (sound == Sound.Clockwise) {
            clockwise.Play();
        }
        if (sound == Sound.AntiClockwise) {
            anticlockwise.Play();
        }
        if (sound == Sound.Hit) {
            hit.Play();
        }
        if (sound == Sound.Theme) {
            theme.Play();
        }
        if (sound == Sound.Clear) {
            if (GameStatusSyncer.Instance.GameTime - lastClear >= 2) {
                GetNewClearIterator();
                clearIterator.Current.Play();
                clearIterator.MoveNext();
            }
            else {
                clearIterator.Current.Play();
                if (!clearIterator.MoveNext()) {
                    GetNewClearIterator();
                }
            }
            lastClear = GameStatusSyncer.Instance.GameTime;
        }
    }

    [Client]
    private void GetNewClearIterator() {
        clearIterator = clear.GetEnumerator();
        clearIterator.MoveNext();
    }
    #endregion

    // Use this for initialization
    private void Start() {
        left = gameObject.AddComponent<AudioSource>();
        right = gameObject.AddComponent<AudioSource>();
        theme = gameObject.AddComponent<AudioSource>();
        clockwise = gameObject.AddComponent<AudioSource>();
        anticlockwise = gameObject.AddComponent<AudioSource>();
        hit = gameObject.AddComponent<AudioSource>();
        clear = new List<AudioSource>();
        for (int i = 1; i <= 5; i++) {
            clear.Add(gameObject.AddComponent<AudioSource>());
        }
    }
}