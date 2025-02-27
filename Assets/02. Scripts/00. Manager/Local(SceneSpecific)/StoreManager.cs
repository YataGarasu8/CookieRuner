using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class StoreManager : MonoBehaviour
{
    [SerializeField] List<string> StoreMent;

    public GameObject YataMent;
    public TextMeshProUGUI YataTalk;

    void Start()
    {
        YataMent.SetActive(false);
    }

    void Update()
    {

    }
    public void StoreYataClick()
    {
        CancelInvoke();
        string yataMent = StoreMent[Random.Range(0, StoreMent.Count)];
        YataTalk.text = yataMent;

        YataMent.SetActive(true);

        Invoke("EraseMentBox", 4.3f);
    }
    public void EraseMentBox()
    {
        YataMent.SetActive(false);
    }
    public void ReturnScene()
    {
        SceneManager.LoadScene("LobbyScene");
    }
}
