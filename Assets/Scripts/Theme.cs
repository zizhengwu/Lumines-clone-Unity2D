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

using UnityEngine;

public class Theme {
    public Sprite Block0;
    public Sprite Block1;
    public Sprite Block0ToBeErased;
    public Sprite Block1ToBeErased;
    public Sprite InsideClearance;
    public string ThemeName;

    public Theme(string name) {
        ThemeName = name;
        string pathPrefix = "Themes/" + ThemeName + "/Sprites/";
        Block0 = Resources.Load<Sprite>(pathPrefix + "block-0");
        Block1 = Resources.Load<Sprite>(pathPrefix + "block-1");
        Block0ToBeErased = Resources.Load<Sprite>(pathPrefix + "block-0-to-be-erased");
        Block1ToBeErased = Resources.Load<Sprite>(pathPrefix + "block-1-to-be-erased");
        InsideClearance = Resources.Load<Sprite>(pathPrefix + "inside-clearance");
    }
}