using System;
using System.Collections.Generic;
//202322158 이준상
public class GameSetting
{
    public static readonly int ROUNDWINING = 3;
    public static readonly int ATTACK = 5;
    public static readonly int DEFANSE = 5;
    public static string LOGINSUCCESS = "Login successful.";
    public static string LOGINERROR = "Login failed.";
}


public enum HandActionType
{
    /** 왼손 뒤집기 */
    SINGLE_HAND_FLIP_LEFT = 1,
    /** 오른손 뒤집기 */
    SINGLE_HAND_FLIP_RIGHT = 2,
    /** 양손 뒤집기 */
    BOTH_HANDS_FLIP = 3,
    /** 찌르기 */
    INSERT_BETWEEN_HANDS = 4,
    /** 가만히 = 기본 */
    SHAKE_OVER_HANDS = 5,
}