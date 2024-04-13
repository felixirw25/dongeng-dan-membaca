using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioMixer mixer;
    public AudioManager audioManager;
    public bool Mute{
        get
        {
            int mute = PlayerPrefs.GetInt("mute", 0);
            return mute == 1;
        }
        set
        {
            PlayerPrefs.SetInt("mute", value ? 1 : 0);
            SetMasterMute(value);
        }
    }
    void Awake(){
        int mute = PlayerPrefs.GetInt("mute", 0);
        Debug.Log(mute);
        SetMasterMute(mute == 1);

        float bgmVolume = PlayerPrefs.GetFloat("BGM_Volume", 1f);
        float effectVolume = PlayerPrefs.GetFloat("SFX_Volume", 1f);

        SetVolume("BGM_Volume", bgmVolume);
        SetVolume("SFX_Volume", effectVolume);
    }

    void Start(){
        if (PlayerPrefs.HasKey("mute"))
        {
            bool mute = PlayerPrefs.GetInt("mute") == 1;
            SetMasterMute(mute);
        }

        if (PlayerPrefs.HasKey("BGM_Volume") && PlayerPrefs.HasKey("SFX_Volume"))
        {
            float bgmVolume = PlayerPrefs.GetFloat("BGM_Volume", 1f);
            float effectVolume = PlayerPrefs.GetFloat("SFX_Volume", 1f);

            SetVolume("BGM_Volume", bgmVolume);
            SetVolume("SFX_Volume", effectVolume);
        }
    }

    public void SetMasterMute(bool mute){
        Debug.Log(mute);
        mixer.SetFloat("MASTER_Volume", mute ? -80f : 0f);
        float test;
        mixer.GetFloat("MASTER_Volume", out test);
        Debug.Log(test);
    }
    public void SetVolume(string tag, float volume){
        Debug.LogFormat("{0} {1}", tag, volume);
        mixer.SetFloat(tag, volume <= 0f ? -80f : Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat(tag, volume);
    }

    public float GetVolume(string tag)
    {
        return PlayerPrefs.GetFloat(tag, 1f);
    }
}
