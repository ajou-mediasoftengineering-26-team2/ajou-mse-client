using UnityEngine;
using UnityEngine.UIElements;

public class ShopUIController : MonoBehaviour
{
    private ShopViewModel _viewModel;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        var roundLabel = root.Q<Label>("Round");
        var moneyLabel = root.Q<Label>("Money");
        var costLabel  = root.Q<Label>("Cost");
        var beforeStat = root.Q<Label>("BeforeStat");
        var afterStat  = root.Q<Label>("AfterStat");
        var upgradeBtn = root.Q<Button>("UpgradeButton");
        // var handImg = root.Q<Image>("HandImg"); // TODO: 이미지 에셋 준비되면 연결

        var idVm = ViewModelLocator.Instance.Get<IDViewModel>();
        _viewModel = new ShopViewModel(idVm.PlayerId.Value, idVm.LobbyId.Value);
        _viewModel.Initialize();

        _viewModel.CurrentRound.Subscribe(round  => roundLabel.text = $"Round {round}");
        _viewModel.Money.Subscribe(money         => moneyLabel.text = money.ToString());
        _viewModel.UpgradeCost.Subscribe(cost    => costLabel.text  = cost.ToString());
        _viewModel.BeforeStat.Subscribe(before   => beforeStat.text = before);
        _viewModel.AfterStat.Subscribe(after     => afterStat.text  = after);
        _viewModel.CanUpgrade.Subscribe(canUpgrade => upgradeBtn.SetEnabled(canUpgrade));
        // _viewModel.HandLevel.Subscribe(lv => handImg.sprite = ...); // TODO

        upgradeBtn.clicked += () => _viewModel.OnUpgrade();
    }

    private void OnDestroy() => _viewModel?.Dispose();
}