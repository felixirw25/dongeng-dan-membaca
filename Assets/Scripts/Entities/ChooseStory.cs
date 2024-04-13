using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChooseStory : MonoBehaviour
{
    public GameObject levelSelector;
    public LevelSelector levelSelectorScript;
    public int indexNumber;
    public void PlayStory(){
        GameObject sceneLoaderCerita = GameObject.FindGameObjectWithTag("SceneLoaderCerita");
        levelSelector = GameObject.FindGameObjectWithTag("LevelSelector");
        levelSelectorScript = levelSelector.GetComponent<LevelSelector>();
        levelSelectorScript.ceritaTerpilih = sceneLoaderCerita.GetComponent<SceneLoaderCerita>().listCeritaInfo[indexNumber-1];
        levelSelectorScript.GetDataCerita();
    }
}
