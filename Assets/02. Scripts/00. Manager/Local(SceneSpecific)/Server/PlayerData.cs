using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 플레이어 데이터를 담을 간단한 클래스 정의
using System;

[Serializable]
public class PlayerData
{
    public int highScore;  // 최고 점수
}

// 일반 데이터 클래스 - json 역직렬화 전용
[Serializable]
public class PlayerDataPlain
{
    public int highScore;
}

// PlayerDataSO must be instantiated using the ScriptableObject.CreateInstance method instead of new PlayerDataSO.
// ScriptableObject를 Newtonsoft.Json으로 직접 역직렬화하려고 할 때 발생
// ScriptableObject는 new 연산자로 인스턴스화하면 안 되고 반드시 ScriptableObject.CreateInstance를 통해 생성해야 하는데,
// Newtonsoft.Json은 내부적으로 new를 호출하려 하기 때문에 이런 오류가 발생한다고 함
