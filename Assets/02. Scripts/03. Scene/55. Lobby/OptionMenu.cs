using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class OptionMenu : MonoBehaviour
{
    public GameObject optionPanel;
    public Slider BGMSlider;
    public Slider SFXSlider;

    private void Start()
    {
        BGMSlider = GameObject.Find("Canvas").transform.Find("OptionPanel").GetComponentsInChildren<Slider>()[0];
        SFXSlider = GameObject.Find("Canvas").transform.Find("OptionPanel").GetComponentsInChildren<Slider>()[1];
    }

    public void ToggleOptionPanel()
    {
        if (optionPanel != null)
        {
            optionPanel.SetActive(!optionPanel.activeSelf);
            SoundManager.Instance.SetBGMVolume(BGMSlider.value);
            SoundManager.Instance.SetSFXVolume(SFXSlider.value);
        }
    }
}
