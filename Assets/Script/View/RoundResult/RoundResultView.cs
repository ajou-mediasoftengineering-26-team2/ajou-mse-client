using UnityEngine;
using UnityEngine.UIElements;

// 202422170 주형준
public class RoundResultView : MonoBehaviour
{
    private Label _roundResult;
    private Label _getMoney;

    private void OnEnable()
    {
        var root     = GetComponent<UIDocument>().rootVisualElement;
        _roundResult = root.Q<Label>("RoundResult");
        _getMoney    = root.Q<Label>("GetMoney");
        //추후에 현재 라운드 표시 필요 -> roundover 수정필요?!
    }

    public void ShowResult(bool isWin)
    {
        _roundResult.text = isWin ? "WIN" : "LOSE";
        _getMoney.text    = "-";
    }
}