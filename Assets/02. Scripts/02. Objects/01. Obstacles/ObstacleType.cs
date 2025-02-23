using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObstacleActionType
{
    Static,      // 서 있는 장애물
    PopUp,      // 제자리에서 위로 튀어나오기
    Disappear,  // 제자리에서 아래로 사라져 길 없어짐
    FlyToPlayer // 맵 멀리서 플레이어를 향해 날아오기
}
