using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionMenu : MonoBehaviour
{
    public GameObject optionPanel;

    public void ToggleOptionPanel()
    {
        if (optionPanel != null)
        {            
            optionPanel.SetActive(!optionPanel.activeSelf);
        }
    }
}
