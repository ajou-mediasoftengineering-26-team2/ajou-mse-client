using UnityEngine;
using UnityEngine.UIElements;

// 202422170 주형준
public class RoundResultView : MonoBehaviour
{
    private Label _currentRound;
    private Label _roundResult;
    private Label _getMoney;

    private void OnEnable()
    {
        var root      = GetComponent<UIDocument>().rootVisualElement;
        _currentRound = root.Q<Label>("CurrentRound");
        _roundResult  = root.Q<Label>("RoundResult");
        _getMoney     = root.Q<Label>("GetMoney");
    }

    public void ShowResult(RoundResultEvent evt)
    {
        var root      = GetComponent<UIDocument>().rootVisualElement;
        _currentRound = root.Q<Label>("CurrentRound");
        _roundResult  = root.Q<Label>("RoundResult");
        _getMoney     = root.Q<Label>("GetMoney");
        
        _currentRound.text = $"Round {evt.currentRound}";
        _roundResult.text  = evt.isWin ? "WIN" : "LOSE";
        _getMoney.text     = $"Coin: {evt.coin}";
    }
}