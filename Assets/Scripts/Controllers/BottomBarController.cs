using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BottomBarController : MonoBehaviour
{
    // Membuat text by text
    // [SerializeField] TextToSpeech tts;
    public TextMeshProUGUI barText;
    public Button nextButton;
    public TextMeshProUGUI personNameText;
    public SceneStory currentScene;
    private State state = State.COMPLETED;
    public BackgroundController backgroundController;

    // Kondisi state sudah selesai ketik atau belum
    private enum State
    {
        PLAYING, COMPLETED
    }

    // Membuat text tidak terlihat
    private void Start(){
        nextButton.gameObject.SetActive(false);
    }

    // 1.a Memutar scene dan 1.b 
    public void PlayScene(SceneStory scene)
    {
        currentScene = scene;
        // tts.ReadText(currentScene.Text);
        PlayNextSentence();
    }

    // 1.b Menampilkan nama pembicara dan efek dialog typewriter
    public void PlayNextSentence()
    {
        StartCoroutine(TypeText(currentScene.Text));
    }

    // Mengecek scene selesai
    public bool IsCompleted()
    {
        return state == State.COMPLETED;
    }

    // Mengecek sentence terakhir dalam 1 scene
    public bool IsLastSentence()
    {
        return true;
    }

    // 1.c Membuat efek typewriter text dan membuat state selesai ketika diklik
    private IEnumerator TypeText(string text)
    {
        nextButton.gameObject.SetActive(false);
        barText.text = "";
        state = State.PLAYING;
        int wordIndex = 0;
        yield return new WaitForSeconds(1f); 

        while (state != State.COMPLETED)
        {
            barText.text += text[wordIndex];
            yield return new WaitForSeconds(0.03f);
            if(++wordIndex == text.Length)
            {
                nextButton.gameObject.SetActive(true);
                yield return new WaitUntil(()=>Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0));
                state = State.COMPLETED;
                break;
            }
        }
    }
}
