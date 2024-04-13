using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; // Memiliki metode convert dari array ke list
using UnityEngine.UI;

public class KataManager : MonoBehaviour
{
    public static KataManager Instance { get; set; }
    [SerializeField] SusunKata wordPrefab;
    [SerializeField] public Transform startSlot, answerSlot;
    [SerializeField] QuizController quizController;
    [SerializeField] public int poinKata, poin;
    public UserData userData;

    void Start()
    {
        Instance = this;
        quizController.resultCanvas.SetActive(false);
        quizController.completedPanel.SetActive(false);
        quizController.failedPanel.SetActive(false);
        quizController.Stop = false;
        quizController.timer = Mathf.Clamp(quizController.timer, 0, quizController.timer);
        userData = GameObject.FindGameObjectWithTag("UserData").GetComponent<UserData>();
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
            SusunKata temp = Instantiate(wordPrefab, startSlot);
            temp.Initialize(startSlot, letterShuffle[i].ToString(), false, quizController);
        }

        // Instantiate the original letters for the answer slot
        for (int i = 0; i < letter.Length; i++)
        {
            SusunKata temp = Instantiate(wordPrefab, answerSlot);
            temp.Initialize(answerSlot, letter[i].ToString(), true, quizController);
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
            // if(quizController.timer >= 40){
            if(quizController.timer >= 0.6*quizController.currentQuiz.Waktu){
                quizController.resultCanvas.SetActive(true);
                quizController.completedPanel.SetActive(true);
                quizController.threeStar.SetActive(true);
                quizController.twoStar.SetActive(false);
                quizController.oneStar.SetActive(false);
                quizController.Stop = true;
                quizController.timer = -1;
                quizController.Correct();
            
                // userData.newPlayerProfile.UpdateStar(quizController.levelSelector.ceritaTerpilih.Id_cerita-1, quizController.levelSelector.levelSelected/3, 3);
                userData.newPlayerProfile.UpdateStar(quizController.levelSelector.ceritaTerpilih.Id_cerita-1, quizController.levelSelector.levelSelected/quizController.levelSelector.variasiLevel, 3);
                userData.WriteUserData();
            }

            // 2 bintang
            else if(quizController.timer < 0.6*quizController.currentQuiz.Waktu && quizController.timer >= 0.3*quizController.currentQuiz.Waktu){
                quizController.resultCanvas.SetActive(true);
                quizController.completedPanel.SetActive(true);
                quizController.threeStar.SetActive(false);
                quizController.twoStar.SetActive(true);
                quizController.oneStar.SetActive(false);
                quizController.Stop = true;
                quizController.timer = -1;
                quizController.Correct();

                if (userData.newPlayerProfile.starPerLevelCerita[quizController.levelSelector.ceritaTerpilih.Id_cerita-1][quizController.levelSelector.levelSelected/quizController.levelSelector.variasiLevel] < 2){
                    userData.newPlayerProfile.UpdateStar(quizController.levelSelector.ceritaTerpilih.Id_cerita-1, quizController.levelSelector.levelSelected/quizController.levelSelector.variasiLevel, 2);
                    userData.WriteUserData();
                }
            }

            //1 bintang
            else if(quizController.timer < 0.3*quizController.currentQuiz.Waktu && quizController.timer > 0){
                quizController.resultCanvas.SetActive(true);
                quizController.completedPanel.SetActive(true);
                quizController.threeStar.SetActive(false);
                quizController.twoStar.SetActive(false);
                quizController.oneStar.SetActive(true);
                quizController.Stop = true;
                quizController.timer = -1;
                quizController.Correct();

                if (userData.newPlayerProfile.starPerLevelCerita[quizController.levelSelector.ceritaTerpilih.Id_cerita-1][quizController.levelSelector.levelSelected/quizController.levelSelector.variasiLevel] < 1){
                    userData.newPlayerProfile.UpdateStar(quizController.levelSelector.ceritaTerpilih.Id_cerita-1, quizController.levelSelector.levelSelected/quizController.levelSelector.variasiLevel, 1);
                    userData.WriteUserData();
                }
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