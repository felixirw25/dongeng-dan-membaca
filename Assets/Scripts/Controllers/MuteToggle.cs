using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MuteToggle : MonoBehaviour
{
    public Toggle toggle;
    [SerializeField] Sprite muteOnSprite;
    [SerializeField] Sprite muteOffSprite;
    // Start is called before the first frame update
    void Start()
    {
        SyncAudio();
    }

    public void SetMute(bool value)
    {
        AudioManager manager = GameSystem.Instance.audioManager;
        manager.Mute = value;
        if (manager.Mute)
            gameObject.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = muteOnSprite;
        else
            gameObject.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = muteOffSprite;
    }

    void SyncAudio()
    {
        AudioManager manager = GameSystem.Instance.audioManager;
        toggle.isOn = manager.Mute;
        if (toggle.isOn)
            gameObject.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = muteOnSprite;
        else
            gameObject.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = muteOffSprite;
    }
}