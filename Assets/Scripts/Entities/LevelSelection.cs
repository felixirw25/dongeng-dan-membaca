using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class LevelSelection : MonoBehaviour
{
    public List<GameObject> levels;
    public Sprite starSprite;
    PlayerProfile data;
    LevelSelector levelSelector;

    public void UpdateLevelImage(){
        int counter = 0;
        levelSelector = GameObject.FindGameObjectWithTag("LevelSelector").GetComponent<LevelSelector>();      
        GameObject.FindGameObjectWithTag("UserData").GetComponent<UserData>().ReadUserDataAsync();
        data = GameObject.FindGameObjectWithTag("UserData").GetComponent<UserData>().newPlayerProfile;
        Debug.Log("Levels: " + levels.Count);
        foreach (GameObject level in levels)
        {
            if(level.transform.GetChild(0).gameObject.activeInHierarchy){
                level.transform.GetChild(1).gameObject.SetActive(false);
                level.transform.GetChild(2).gameObject.SetActive(false);
                level.transform.GetChild(3).gameObject.SetActive(false);
            }
            else{
                for (int i = 0; i < data.starPerLevelCerita[levelSelector.ceritaTerpilih.Id_cerita-1][counter]; i++){
                    level.transform.GetChild(i + 1).gameObject.SetActive(true);
                    level.transform.GetChild(i + 1).gameObject.GetComponent<Image>().sprite = starSprite;
                }
            }
            counter++;  
        }
    }
}