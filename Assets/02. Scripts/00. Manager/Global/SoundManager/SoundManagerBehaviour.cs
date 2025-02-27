using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// 설명: MonoBehaviour를 상속하는 헬퍼 클래스
// SoundManager에서 사용할 수 없는 코루틴 및 transform 관련 기능을 제공하기 위해 사용
// 싱글톤 패턴을 적용하여 중복 생성을 방지하고, 씬 전환 시에도 유지

public class SoundManagerBehaviour : MonoBehaviour
{
    // 싱글톤 인스턴스를 저장하는 정적 변수
    private static SoundManagerBehaviour instance;

    // 외부에서 접근 가능한 싱글톤 인스턴스 프로퍼티
    public static SoundManagerBehaviour Instance
    {
        get
        {
            // 인스턴스가 존재하지 않으면 생성
            if (instance == null)
            {
                var obj = new GameObject("SoundManagerHelper"); // GameObject 생성
                instance = obj.AddComponent<SoundManagerBehaviour>(); // 컴포넌트 추가
                DontDestroyOnLoad(obj); // 씬 전환 시에도 파괴되지 않도록 설정
            }
            return instance; // 인스턴스 반환
        }
    }

    // SoundManager에서 코루틴 실행 시 호출하는 함수
    public void RunCoroutine(System.Collections.IEnumerator coroutine)
    {
        StartCoroutine(coroutine); // 전달된 코루틴 실행
    }

    private void Awake()
    {
        
    }

    private void Start()
    {
        SoundManager.Instance.LoadSounds();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameScene")
        {
            SoundManager.Instance.StopBGM();
            SoundManager.Instance.PlayBGM("GameSceneBGM01");
        }
        if(scene.name == "LobbyScene")
        {
            SoundManager.Instance.StopBGM();
            SoundManager.Instance.PlayBGM("LobbyBGM01");
        }
        if(scene.name == "StoreScene")
        {
            SoundManager.Instance.StopBGM();
            SoundManager.Instance.PlayBGM("StoreSceneBGM01");
        }
    }
}
