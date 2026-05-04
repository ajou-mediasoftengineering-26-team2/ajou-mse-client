using System.Collections.Generic;
using UnityEngine;
//202322158 이준상
[System.Serializable]
public class HandActionData
{
    public string actionName;    // 액션 이름 (예: "One Hand Flip")
    public HandActionType actionCode;      // 고정 코드 (예: 101, 102...)
    public Sprite actionImage;  // UI에 보여줄 이미지
}

[CreateAssetMenu(fileName = "ActionDatabase", menuName = "Custom/ActionData")] 
public class ActionDatabase : ScriptableObject
{
    public List<HandActionData> actions;
}