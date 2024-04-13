using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuizTypeAcakKata : MonoBehaviour
{
    [SerializeField] Text waktu_salah_text;
    public AcakKataGameController quizController;
    private SceneQuiz theQuiz;
    // Jenis Kuis
    public enum QuizType
    {
        Menyusun
    }
    private float damage;
    public float Damage { get => damage; set => damage = value; }

    // 2.b.iii Menampilkan opsi jawaban sesuai jenis soal
    public void checkQuizType(SceneQuiz quiz){
        theQuiz = quiz;
        damage = theQuiz.Waktu_Salah;
        waktu_salah_text.text = "-"+damage.ToString();
        Debug.Log("theQuiz.Answer:"+theQuiz.Answer);
        AcakKata.Instance.InitKata(theQuiz.Answer);
    }
}