using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderMixer : MonoBehaviour
{
    public Slider slider;

    public string mixerGroup;

    void Start()
    {
        SyncSlider();
    }

    public void OnChangeVolume(float value)
    {
        AudioManager manager = GameSystem.Instance.audioManager;
        manager.SetVolume(mixerGroup, value);
    }

    public void SyncSlider()
    {
        if (slider == null) 
            return;

        AudioManager manager = GameSystem.Instance.audioManager;
        slider.value = manager.GetVolume(mixerGroup);
    }
}