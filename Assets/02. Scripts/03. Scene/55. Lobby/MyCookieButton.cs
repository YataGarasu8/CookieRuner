using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MyCookieButton : MonoBehaviour
{
    // Start is called before the first frame update
    public void LoadMyCookieScene()
    {
        SceneManager.LoadScene("MyCookieScene");
    }

   
}
