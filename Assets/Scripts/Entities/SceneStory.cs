using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SceneStory
{
    // Setiap scene memiliki background dan list kalimat
    [TextArea(3,5)]
    private string text;
    private string backgroundLink;
    private string audioLink;
    private string musicLink;
    private int id_adegan;
    private SceneStory nextScene;

    public string Text { get => text; set => text = value; }
    public string BackgroundLink { get => backgroundLink; set => backgroundLink = value; }
    public string AudioLink { get => audioLink; set => audioLink = value; }
    public string MusicLink { get => musicLink; set => musicLink = value; }
    public int Id_adegan { get => id_adegan; set => id_adegan = value; }
    public SceneStory NextScene { get => nextScene; set => nextScene = value; }
}
