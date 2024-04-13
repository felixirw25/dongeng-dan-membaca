using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour {
 
    [SerializeField] GameController gameController;
    public AudioClip NewMusic;
    public GameObject audioManager;

    [Obsolete]
    public void LoadMusic(SceneStory currentScene){
        audioManager = GameObject.Find("AudioManager");
        if(gameController.currentScene1.MusicLink != ""){
            Debug.Log("Berhasil memutar:"+currentScene.MusicLink.ToString().Trim());
            StartCoroutine(LoadMusicIE(currentScene.MusicLink.ToString().Trim()));
        }
    }
    [Obsolete]
    IEnumerator LoadMusicIE(string mediaUrl){
        UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(mediaUrl, AudioType.MPEG);
        yield return request.SendWebRequest(); 
        if(request.isNetworkError || request.isHttpError){
            Debug.Log(request.error);
        }
        else{
            AudioClip myClip = DownloadHandlerAudioClip.GetContent(request);
            audioManager.GetComponent<AudioSource>().clip = myClip;
            audioManager.GetComponent<AudioSource>().Play();
        }
    }
    [Obsolete]
    public IEnumerator LoadAudioIE(string audioUrl){
        AudioSource musicManager = GameObject.Find("MusicManager").GetComponent<AudioSource>();
        UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(audioUrl, AudioType.MPEG);
        yield return request.SendWebRequest(); 
        if(request.isNetworkError || request.isHttpError){
            Debug.Log(request.error);
        }
        else{
            AudioClip audioClip = DownloadHandlerAudioClip.GetContent(request);
            musicManager.clip = audioClip;
            musicManager.Play();
        }
    }
}