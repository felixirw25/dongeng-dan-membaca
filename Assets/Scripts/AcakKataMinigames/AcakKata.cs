using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; // Memiliki metode convert dari array ke list
using UnityEngine.UI;

public class AcakKata : MonoBehaviour
{
    public static AcakKata Instance { get; set; }
    [SerializeField] SusunKataManager wordPrefab;
    [SerializeField] public Transform startSlot, answerSlot;
    [SerializeField] AcakKataManager acakKataManager;
    [SerializeField] public int poinKata, poin;

    void Start()
    {
        Instance = this;
        acakKataManager.resultCanvas.SetActive(false);
        acakKataManager.completedPanel.SetActive(false);
        acakKataManager.failedPanel.SetActive(false);
        acakKataManager.Stop = false;
        acakKataManager.timer = Mathf.Clamp(acakKataManager.timer, 0, acakKataManager.timer);
    }
    
    public void InitKata(string word){
        poinKata = 0;
        poin = 0;
        char[] letter = word.ToCharArray();
        char[] letterShuffle = new char[letter.Length];

        List<char> letterCopy =  new List<char>();
        letterCopy = letter.ToList(); // membutuhkan System.Linq

        string result = "";

        // Shuffle the letters without recursion
        while (result == word || result == "")
        {
            letterCopy.Shuffle(); // Shuffle the list using your custom shuffle method
            letterCopy.CopyTo(letterShuffle);
            result = new string(letterShuffle);
        }

        // Instantiate the shuffled letters
        for (int i = 0; i < letter.Length; i++)
        {
            SusunKataManager temp = Instantiate(wordPrefab, startSlot);
            temp.Initialize(startSlot, letterShuffle[i].ToString(), false, acakKataManager);
        }

        // Instantiate the original letters for the answer slot
        for (int i = 0; i < letter.Length; i++)
        {
            SusunKataManager temp = Instantiate(wordPrefab, answerSlot);
            temp.Initialize(answerSlot, letter[i].ToString(), true, acakKataManager);
        }

        poinKata = letter.Length;
    }

    public void ResetPoin(){
        poin=0;
    }

    public void TambahPoin(){
        poin++;

        if (poin == poinKata){
            // 3 bintang
            if(acakKataManager.timer >= 0.6*acakKataManager.currentQuiz.Waktu){
                acakKataManager.resultCanvas.SetActive(true);
                acakKataManager.completedPanel.SetActive(true);
                acakKataManager.threeStar.SetActive(true);
                acakKataManager.twoStar.SetActive(false);
                acakKataManager.oneStar.SetActive(false);
                acakKataManager.Stop = true;
                acakKataManager.timer = -1;
                acakKataManager.Correct();
            }

            // 2 bintang
            else if(acakKataManager.timer < 0.6*acakKataManager.currentQuiz.Waktu && acakKataManager.timer >= 0.3*acakKataManager.currentQuiz.Waktu){
                acakKataManager.resultCanvas.SetActive(true);
                acakKataManager.completedPanel.SetActive(true);
                acakKataManager.threeStar.SetActive(false);
                acakKataManager.twoStar.SetActive(true);
                acakKataManager.oneStar.SetActive(false);
                acakKataManager.Stop = true;
                acakKataManager.timer = -1;
                acakKataManager.Correct();
            }

            //1 bintang
            else if(acakKataManager.timer < 0.3*acakKataManager.currentQuiz.Waktu && acakKataManager.timer > 0){
                acakKataManager.resultCanvas.SetActive(true);
                acakKataManager.completedPanel.SetActive(true);
                acakKataManager.threeStar.SetActive(false);
                acakKataManager.twoStar.SetActive(false);
                acakKataManager.oneStar.SetActive(true);
                acakKataManager.Stop = true;
                acakKataManager.timer = -1;
                acakKataManager.Correct();
            }

            GameObject answerSlotBefore = GameObject.FindGameObjectWithTag("answerSlot");
            if(answerSlotBefore != null){
                for(int i = 0; i < answerSlotBefore.transform.childCount; i++){
                    Destroy(answerSlotBefore.transform.GetChild(i).gameObject);   
                }
            }
        }
    }
}