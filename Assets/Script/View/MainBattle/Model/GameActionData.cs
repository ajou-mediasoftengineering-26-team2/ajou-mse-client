using System.Collections.Generic;
using UnityEngine;
//202322158 이준상


/// <summary>
/// HandData Action
/// </summary>
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


/// <summary>
/// Classe used to show your choices when it's your turn.
/// </summary>
public static class ActionDatabase
{
    public static List<HandActionData> AttackActions { get; private set; }
    public static List<HandActionData> DefendActions { get; private set; }

    static ActionDatabase()
    {
        // init attack action data 
        AttackActions = new List<HandActionData>
        {
            new HandActionData("Left", HandActionType.SINGLE_HAND_FLIP_LEFT, "Left"),
            new HandActionData("Right", HandActionType.SINGLE_HAND_FLIP_RIGHT, "Right"),
            new HandActionData("Both", HandActionType.BOTH_HANDS_FLIP, "Both"),
            new HandActionData("Stab", HandActionType.INSERT_BETWEEN_HANDS, "slice"),
            new HandActionData("Wave", HandActionType.SHAKE_OVER_HANDS, "ok")
        };

        // init defend action data
        DefendActions = new List<HandActionData>
        {
            new HandActionData("Left", HandActionType.SINGLE_HAND_FLIP_LEFT, "Left"),
            new HandActionData("Right", HandActionType.SINGLE_HAND_FLIP_RIGHT, "Right"),
            new HandActionData("Both", HandActionType.BOTH_HANDS_FLIP, "Both"),
            new HandActionData("Defense", HandActionType.INSERT_BETWEEN_HANDS, "None"),
            new HandActionData("Pause", HandActionType.SHAKE_OVER_HANDS, "ok")
        };
    }

    /// <summary>
    /// Help function to import images from Resources folder
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static Sprite GetActionSprite(string path)
    {
        return Resources.Load<Sprite>(path);
    }
}