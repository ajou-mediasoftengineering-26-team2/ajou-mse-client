using UnityEngine;
using UnityEngine.UIElements;

// 202422170 주형준
public class RoundResultView : MonoBehaviour
{
    private RoundResultViewModel _viewModel;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        var currentRound = root.Q<Label>("CurrentRound");
        var roundResult  = root.Q<Label>("RoundResult");
        var getMoney     = root.Q<Label>("GetMoney");

        _viewModel = new RoundResultViewModel();
        _viewModel.SetPlayerInfo(SceneDataBridge.playerId, SceneDataBridge.MatchId);
        _viewModel.Initialize();

        root.style.display = DisplayStyle.None;

        _viewModel.IsVisible.Subscribe(visible =>
            root.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None);

        _viewModel.CurrentRound.Subscribe(v => currentRound.text = v ?? "");

        _viewModel.IsWin.Subscribe(isWin =>
            roundResult.text = isWin ? "WIN" : "LOSE");

        _viewModel.GetMoney.Subscribe(amount =>
            getMoney.text = amount.ToString());
    }

    private void OnDisable() => _viewModel?.Dispose();
}