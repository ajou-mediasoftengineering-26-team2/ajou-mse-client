using System.Collections.Generic;
using UnityEngine;
//202322158 이준상
[System.Serializable]
public class HandActionData
{
    public string actionName;
    public HandActionType actionCode;
    public string imagePath; // 리소스 폴더 내의 경로

    public HandActionData(string name, HandActionType code, string path)
    {
        actionName = name;
        actionCode = code;
        imagePath = path;
    }
}


public static class ActionDatabase
{
    public static List<HandActionData> AttackActions { get; private set; }
    public static List<HandActionData> DefendActions { get; private set; }

    static ActionDatabase()
    {
        // 공격 액션 데이터 초기화
        AttackActions = new List<HandActionData>
        {
            new HandActionData("Left", HandActionType.SINGLE_HAND_FLIP_LEFT, "Left"),
            new HandActionData("Right", HandActionType.SINGLE_HAND_FLIP_RIGHT, "Right"),
            new HandActionData("Both", HandActionType.BOTH_HANDS_FLIP, "Both"),
            new HandActionData("Stab", HandActionType.INSERT_BETWEEN_HANDS, "slice"),
            new HandActionData("Wave", HandActionType.SHAKE_OVER_HANDS, "ok")
        };

        // 방어 액션 데이터 초기화
        DefendActions = new List<HandActionData>
        {
            new HandActionData("Left", HandActionType.SINGLE_HAND_FLIP_LEFT, "Left"),
            new HandActionData("Right", HandActionType.SINGLE_HAND_FLIP_RIGHT, "Right"),
            new HandActionData("Both", HandActionType.BOTH_HANDS_FLIP, "Both"),
            new HandActionData("Defense", HandActionType.INSERT_BETWEEN_HANDS, "None"),
            new HandActionData("Pause", HandActionType.SHAKE_OVER_HANDS, "ok")
        };
    }

    // 이미지를 Resources 폴더에서 불러오는 헬퍼 함수
    public static Sprite GetActionSprite(string path)
    {
        return Resources.Load<Sprite>(path);
    }
}