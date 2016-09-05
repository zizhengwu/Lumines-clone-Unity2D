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