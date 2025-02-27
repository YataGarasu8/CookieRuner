using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using TMPro;


public class MentManager : MonoBehaviour
{
    [SerializeField] List<string> CookieMent;  // 랜덤 대사 리스트

    public GameObject PlayerMent;  // 말풍선 UI 오브젝트
    public TextMeshProUGUI PlayerTalk;  // 말풍선 텍스트 컴포넌트

    public Button playerButton;  // Player 캐릭터 버튼

    void Start()
    {
        // 말풍선 초기 상태는 비활성화
        PlayerMent.SetActive(false);
        if (CookieMent == null)
        {
            CookieMent = new List<string>();
        }

        if (CookieMent.Count == 0) // 비어 있을 때 기본 대사 추가
        {
            CookieMent.Add("저희 조는 마개조 입니다!");
            CookieMent.Add("안녕하세요~");
            CookieMent.Add("좋은 하루 되세요!");
        }

        // 버튼 클릭 이벤트 연결
        playerButton.onClick.AddListener(LobbyPlayerClick);
    }

    // 말풍선에 랜덤 대사를 출력
    public void LobbyPlayerClick()
    {
        string playerMent = CookieMent[Random.Range(0, CookieMent.Count)];  // 랜덤 대사 선택
        PlayerTalk.text = playerMent;  // 텍스트 업데이트

        PlayerMent.SetActive(true);  // 말풍선 표시

        Invoke("EraseMentBox", 10f);  // 3.3초 후 말풍선 숨기기
    }

    // 말풍선 숨기기
    public void EraseMentBox()
    {
        PlayerMent.SetActive(false);  // 말풍선 비활성화
    }

}
