using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundButton : MonoBehaviour
{
    [SerializeField] AudioSource buttonAudio;
    [SerializeField] AudioClip buttonClip;
    [SerializeField] AudioClip sliderClip;

    public void Start(){
        if(buttonAudio == null || buttonClip == null || sliderClip == null){
            return;
        }
    }
    public void PlaySoundButton()
    {
        if(sliderClip){
            buttonAudio.PlayOneShot(sliderClip);
        }
        else
            buttonAudio.PlayOneShot(buttonClip);
    }
    public void Move(string scene){
        SceneManager.LoadScene(scene);
    }
}
