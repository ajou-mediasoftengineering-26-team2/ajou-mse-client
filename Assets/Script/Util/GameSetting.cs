using System;
using System.Collections.Generic;
//202322158 이준상
public class GameSetting
{
    public static readonly int ROUNDWINING = 3;
    public static readonly int ATTACK = 5;
    public static readonly int DEFANSE = 5;
    public static string LOGINSUCCESS = "로그인에 성공하셨습니다.";
    public static string LOGINERROR = "로그인에 실패하셧습니다.";
}


public enum HandActionType
{
    None = 0,                // 미선택 시 자동 (가만히)
    SingleHandFlipLeft = 1,  // 왼손 뒤집기
    SingleHandFlipRight = 2, // 오른손 뒤집기
    BothHandsFlip = 3,       // 양손 뒤집기
    InsertBetweenHands = 4,  // 찌르기
    ShakeOverHands = 5       // 손 위 흔들기
}