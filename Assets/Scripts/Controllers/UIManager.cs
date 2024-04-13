 using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text starsText;
    public TextMeshProUGUI hintsText;
    public UserData userData;
    LevelSelector levelSelector;

    private void start(){
        if(starsText==null || hintsText==null){
            return;
        }
    }
    private void Update()
    {
        if(hintsText!=null){
            // UpdateHintsUI();
            return;
        }
    }

    public void UpdateStarsUI()
    {
        userData = GameObject.FindGameObjectWithTag("UserData").GetComponent<UserData>();
        levelSelector = GameObject.FindGameObjectWithTag("LevelSelector").GetComponent<LevelSelector>();
        LevelSelection levelSelection = gameObject.GetComponent<LevelSelection>();
        starsText.text = "" + userData.newPlayerProfile.CountStarCerita(levelSelector.ceritaTerpilih.Id_cerita-1) + "/" + 
        levelSelection.levels.Count * 3;
    }
    public void UpdateHintsUI()
    {
        hintsText.text = PlayerPrefs.GetInt("total_hint", 3).ToString()+"x";
    }
}
