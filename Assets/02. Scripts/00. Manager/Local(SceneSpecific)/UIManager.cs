using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    static UIManager instance;

    private Stack<GameObject> uiStack;

    public static UIManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UIManager>();

                if (instance == null)
                {
                    GameObject obj = new GameObject("UIManager");
                    instance = obj.AddComponent<UIManager>();
                    DontDestroyOnLoad(obj);
                }
            }
            return instance;
        }
    }

    public Stack<GameObject> UIStack => uiStack;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject.transform.root.gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        uiStack = new Stack<GameObject>();
    }

    public void OpenUI(GameObject newUI)
    {
        if (uiStack.Count > 0)
        {
            uiStack.Peek().SetActive(false);
        }
        Debug.Log(newUI.name);
        newUI.SetActive(true);
        uiStack.Push(newUI);
    }

    public void CloseUI()
    {
        if (uiStack.Count > 0)
        {
            GameObject currentUI = uiStack.Pop();
            currentUI.SetActive(false);

            if (uiStack.Count > 0)
            {
                uiStack.Peek().SetActive(true);
            }
        }
    }

    //UI 오브젝트의 하위 텍스트 객체를 찾아내서 원하는 텍스트로 변경해줌
    public void ChangeText(GameObject newUI, string textUIName, string text, Color? color = null, int textSize = 11)
    {
        TextMeshProUGUI[] textUI = newUI.GetComponentsInChildren<TextMeshProUGUI>();
        TextMeshProUGUI[] search = textUI.Where(x => x.name == textUIName).ToArray();
        search[0].text = text;
        search[0].color = color.HasValue ? color.Value : Color.black;
        search[0].fontSize = textSize;
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UIManager.Instance.UIStack.Clear();
    }
}
