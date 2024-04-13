using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuizTypes : MonoBehaviour
{
    [SerializeField] Button[] buttonPilihanPilgans;
    [SerializeField] public TMP_InputField jawabanMengisi;
    [SerializeField] Text waktu_salah_text;
    public QuizController quizController;
    private SceneQuiz theQuiz;
    // Jenis Kuis
    public enum QuizType
    {
        Pilgan,
        Menyusun,
        Mengisi,
    }
    // [SerializeField] private float baseDamage = 5f;
    private float damage;
    public float Damage { get => damage; set => damage = value; }
    public UserData userData;
    
    // private void OnEnable(){
    //     damage = baseDamage;
    // }

    // 2.b.iii Menampilkan opsi jawaban sesuai jenis soal
    public void checkQuizType(SceneQuiz quiz){
        theQuiz = quiz;
        damage = quiz.Waktu_Salah;
        waktu_salah_text.text = "-"+damage.ToString();
        userData = GameObject.FindGameObjectWithTag("UserData").GetComponent<UserData>();
        if(quiz.QuizType=="Pilgan"){
            for (int i=0; i<buttonPilihanPilgans.Length; i++){
                buttonPilihanPilgans[i].GetComponentInChildren<TextMeshProUGUI>().text = quiz.PilihanPilgan[i];
            }
            foreach (var buttonPilihanPilgan in buttonPilihanPilgans){
                buttonPilihanPilgan.onClick.RemoveAllListeners();
                buttonPilihanPilgan.onClick.AddListener(()=>{
                    
                    // 3 bintang
                    if(quizController.timer >= 0.6*quizController.currentQuiz.Waktu && Equals(quiz.Answer, buttonPilihanPilgan.GetComponentInChildren<TextMeshProUGUI>().text)){
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
                    else if(quizController.timer < 0.6*quizController.currentQuiz.Waktu && quizController.timer >= 0.3*quizController.currentQuiz.Waktu && Equals(quiz.Answer, buttonPilihanPilgan.GetComponentInChildren<TextMeshProUGUI>().text)){
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
                    else if(quizController.timer < 0.3*quizController.currentQuiz.Waktu && quizController.timer > 0 && Equals(quiz.Answer, buttonPilihanPilgan.GetComponentInChildren<TextMeshProUGUI>().text)){
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
                    else if(Equals(quiz.Answer, buttonPilihanPilgan.GetComponentInChildren<TextMeshProUGUI>().text)==false){
                        quizController.timer -= damage;
                        quizController.Wrong();
                        return;
                    }
                });
            }
        }
        else if(quiz.QuizType=="Menyusun"){
            KataManager.Instance.InitKata(quiz.Answer);
        }
    }
    public void ClearButton(){
        string emptyAnswer = "";  
        jawabanMengisi.text = emptyAnswer;
    }
    public void SubmitButton(){
        userData = GameObject.FindGameObjectWithTag("UserData").GetComponent<UserData>();
        if(theQuiz != null){
                Debug.Log("theQuiz.Answer.ToString().ToLower():" + theQuiz.Answer.ToString().ToLower().TrimEnd());
                Debug.Log("jawabanMengisi.text.ToString().ToLower().TrimEnd():" + jawabanMengisi.text.ToString().ToLower().TrimEnd());
            if(theQuiz.Answer.ToString().ToLower().TrimEnd() == jawabanMengisi.text.ToString().ToLower().TrimEnd()){
                // 3 bintang
                if(quizController.timer >= 0.6*quizController.currentQuiz.Waktu){
                    quizController.resultCanvas.SetActive(true);
                    quizController.completedPanel.SetActive(true);
                    quizController.threeStar.SetActive(true);
                    quizController.twoStar.SetActive(false);
                    quizController.oneStar.SetActive(false);
                    quizController.Stop = true;
                    quizController.timer = -1;
                    quizController.Correct();

                    userData.newPlayerProfile.UpdateStar(quizController.levelSelector.ceritaTerpilih.Id_cerita-1, quizController.levelSelector.levelSelected/quizController.levelSelector.variasiLevel, 3);
                    userData.WriteUserData();
                    jawabanMengisi.text = "";           
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
                    jawabanMengisi.text = "";
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
                    jawabanMengisi.text = "";
                }
            }
            else if(jawabanMengisi.text.ToString()==""){
                return;
            }else{
                quizController.timer -= damage;
                quizController.Wrong();
                return;
            }
        }
    }
}
