using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using UnityEngine.Networking;

public class AcakKataGameController : MonoBehaviour
{
    public AcakKataManager currentQuiz;
    public GameObject quizCanvas;
    private bool quizTime = false;
    public bool QuizTime { get => quizTime; set => quizTime = value; }
    public GameObject PausePanel;
    public Button PauseButton;
    public Button pauseRestart;
    public Button closeHintButton;
    public GameObject hintPanel;
    public Button HintButton;
    public PlayerProfile playerProfile;
    public bool GamePause;
    public SceneLoader sceneLoader;
    public UserData userData;
    bool update;
    // public IklanManager iklanManager;
    public FirebaseAuth auth;

    // 1. Memutar scene
    [Obsolete]
    void Start()
    {
        // StopAllCoroutines();
        QuizTime = false;
        update = false;
        quizCanvas.SetActive(true);
        hintPanel.SetActive(false);
        PausePanel.SetActive(false);

        HintButton.onClick.AddListener(()=>
        {
            Hint();
        });

        closeHintButton.onClick.AddListener(()=>
        {
            closeHint();
        });
    }

    // 2.b Menampilkan quiz
    public void ShowQuiz()
    {
        currentQuiz.Indeks = currentQuiz.Indeks+1;
        StartCoroutine(currentQuiz.LoadCover(1f));
        StartCoroutine(currentQuiz.PlayQuiz(currentQuiz.Indeks));
    }

    // 3. Pausecontrol
    public void PauseControl()
    {
        if (Time.timeScale == 1)
        {
            PausePanel.SetActive(true);
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
            PausePanel.SetActive(false);
        }
    }

    // hint
    public void Hint()
    {
        hintPanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void closeHint()
    {
        hintPanel.SetActive(false);
        Time.timeScale = 1;
    }
}