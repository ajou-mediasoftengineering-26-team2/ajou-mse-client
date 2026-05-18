using UnityEngine;
using UnityEngine.UIElements;

// 202422170 주형준
public class PerkAndShopView : MonoBehaviour
{
    private PerkAndShopViewModel _viewModel;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        // header
        var currentRoundLabel = root.Q<Label>("CurrentRound");
        var moneyLabel        = root.Q<Label>("Money");

        // perk buttons
        var perk1Btn = root.Q<Button>("SelectBtn1");
        var perk2Btn = root.Q<Button>("SelectBtn2");
        var perk3Btn = root.Q<Button>("SelectBtn3");

        // perk labels
        var perk1Title = root.Q<Label>("Perk1Title");
        var perk2Title = root.Q<Label>("Perk2Title");
        var perk3Title = root.Q<Label>("Perk3Title");
        var perk1Exp   = root.Q<Label>("Perk1Exp");
        var perk2Exp   = root.Q<Label>("Perk2Exp");
        var perk3Exp   = root.Q<Label>("Perk3Exp");

        // upgrade card
        var beforeInfo  = root.Q<Label>("BeforeInfo");
        var afterInfo   = root.Q<Label>("AfterInfo");
        var upgradeCost = root.Q<Label>("UpgradeCost");
        var upgradeBtn  = root.Q<Button>("UpgradeBtn");

        // grab IDs from LoginViewModel, then init
        var idVm = ViewModelLocator.Instance.Get<LoginViewModel>();
        _viewModel = new PerkAndShopViewModel();
        _viewModel.SetPlayerInfo(idVm.PlayerId.Value, idVm.LobbyId.Value);
        _viewModel.Initialize();

        // bind observables
        _viewModel.CurrentRound.Subscribe(v => currentRoundLabel.text = $"Round {v}");
        _viewModel.Money.Subscribe(v        => moneyLabel.text        = $"Money: {v}");

        _viewModel.Perk1Title.Subscribe(v => perk1Title.text = v ?? "");
        _viewModel.Perk2Title.Subscribe(v => perk2Title.text = v ?? "");
        _viewModel.Perk3Title.Subscribe(v => perk3Title.text = v ?? "");
        _viewModel.Perk1Desc.Subscribe(v  => perk1Exp.text  = v ?? "");
        _viewModel.Perk2Desc.Subscribe(v  => perk2Exp.text  = v ?? "");
        _viewModel.Perk3Desc.Subscribe(v  => perk3Exp.text  = v ?? "");

        _viewModel.BeforeStat.Subscribe(v  => beforeInfo.text  = v ?? "");
        _viewModel.AfterStat.Subscribe(v   => afterInfo.text   = v ?? "");
        _viewModel.UpgradeCost.Subscribe(v => upgradeCost.text = $"{v}G");
        _viewModel.CanUpgrade.Subscribe(v  => upgradeBtn.SetEnabled(v));

        // disable perk buttons once one is picked
        _viewModel.CanSelect.Subscribe(v =>
        {
            perk1Btn.SetEnabled(v);
            perk2Btn.SetEnabled(v);
            perk3Btn.SetEnabled(v);
        });

        // button events
        upgradeBtn.clicked += () => _viewModel.OnUpgrade();
        perk1Btn.clicked   += () => _viewModel.OnSelectPerk(1);
        perk2Btn.clicked   += () => _viewModel.OnSelectPerk(2);
        perk3Btn.clicked   += () => _viewModel.OnSelectPerk(3);
    }

    private void OnDestroy() => _viewModel?.Dispose();
}