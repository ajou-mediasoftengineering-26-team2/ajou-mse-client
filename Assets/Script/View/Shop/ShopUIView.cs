using UnityEngine;
using UnityEngine.UIElements;

// 202422170 주형준
public class ShopUIView : MonoBehaviour
{
    private ShopViewModel _viewModel;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        // Query UI elements from the UXML document
        var roundLabel = root.Q<Label>("Round");
        var moneyLabel = root.Q<Label>("Money");
        var costLabel  = root.Q<Label>("Cost");
        var beforeStat = root.Q<Label>("BeforeStat");
        var afterStat  = root.Q<Label>("AfterStat");
        var upgradeBtn = root.Q<Button>("UpgradeButton");
        // var handImg = root.Q<Image>("HandImg"); // 이미지 에셋 준비되면 연결

        var idVm = ViewModelLocator.Instance.Get<LoginViewModel>();
        _viewModel = new ShopViewModel(idVm.PlayerId.Value, idVm.LobbyId.Value);
        _viewModel.Initialize();

        // Subscribe to ViewModel observables and update UI labels
        _viewModel.CurrentRound.Subscribe(round  => roundLabel.text = $"Round {round}");
        _viewModel.Money.Subscribe(money         => moneyLabel.text = money.ToString());
        _viewModel.UpgradeCost.Subscribe(cost    => costLabel.text  = cost.ToString());
        _viewModel.BeforeStat.Subscribe(before   => beforeStat.text = before);
        _viewModel.AfterStat.Subscribe(after     => afterStat.text  = after);
        _viewModel.CanUpgrade.Subscribe(canUpgrade => upgradeBtn.SetEnabled(canUpgrade));
    
        // Trigger upgrade logic via ViewModel on button click
        upgradeBtn.clicked += () => _viewModel.OnUpgrade();
    }

    // Dispose ViewModel to prevent memory leaks
    private void OnDestroy() => _viewModel?.Dispose();
}