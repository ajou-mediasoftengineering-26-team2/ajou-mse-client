using System;
using UnityEngine;
using UnityEngine.UIElements;

// 202422170 주형준
public class PerkAndShopView : MonoBehaviour
{
    private PerkAndShopViewModel _viewModel;

    private void OnEnable()
    {
        //Setup();
    }

    public void Setup()
    {
        _viewModel?.Dispose();
        _viewModel = null;

        var root = GetComponent<UIDocument>().rootVisualElement;

        var selectBtn1   = root.Q<Button>("SelectBtn1");
        var selectBtn2   = root.Q<Button>("SelectBtn2");
        var selectBtn3   = root.Q<Button>("SelectBtn3");
        var perk1Title   = root.Q<Label>("Perk1Title");
        var perk1Exp     = root.Q<Label>("Perk1Exp");
        var perk2Title   = root.Q<Label>("Perk2Title");
        var perk2Exp     = root.Q<Label>("Perk2Exp");
        var perk3Title   = root.Q<Label>("Perk3Title");
        var perk3Exp     = root.Q<Label>("Perk3Exp");
        var perk1Img     = root.Q<Image>("Perk1Img");
        var perk2Img     = root.Q<Image>("Perk2Img");
        var perk3Img     = root.Q<Image>("Perk3Img");
        var handImg      = root.Q<Image>("HandImg");
        var beforeInfo   = root.Q<Label>("BeforeInfo");
        var afterInfo    = root.Q<Label>("AfterInfo");
        var upgradeCost  = root.Q<Label>("UpgradeCost");
        var upgradeBtn   = root.Q<Button>("UpgradeBtn");
        var currentRound = root.Q<Label>("CurrentRound");
        var money        = root.Q<Label>("Money");
        var timer        = root.Q<VisualElement>("Timer");
        var timerImg     = root.Q<Image>("TimerImg");

        _viewModel = new PerkAndShopViewModel();
        _viewModel.SetPlayerInfo(SceneDataBridge.playerId, SceneDataBridge.MatchId);
        _viewModel.Initialize();
        root.style.display = DisplayStyle.None;
        _viewModel.IsVisible.Subscribe(visible =>
            root.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None);

        if (timerImg != null)
            timerImg.sprite = Resources.Load<Sprite>("Pixel Clock/TimerImg");

        _viewModel.TimerRatio.Subscribe(ratio =>
        {
            if (timer != null)
                timer.style.width = new Length(ratio * 100, LengthUnit.Percent);
        });

        _viewModel.RoundLabel.Subscribe(v  => { if (currentRound != null) currentRound.text = v ?? ""; });
        _viewModel.CoinLabel.Subscribe(v   => { if (money        != null) money.text        = $"Money: {v}"; });

        _viewModel.Perk1Title.Subscribe(v => { if (perk1Title != null) perk1Title.text = v ?? ""; });
        _viewModel.Perk1Desc.Subscribe(v  => { if (perk1Exp   != null) perk1Exp.text   = v ?? ""; });
        _viewModel.Perk2Title.Subscribe(v => { if (perk2Title != null) perk2Title.text = v ?? ""; });
        _viewModel.Perk2Desc.Subscribe(v  => { if (perk2Exp   != null) perk2Exp.text   = v ?? ""; });
        _viewModel.Perk3Title.Subscribe(v => { if (perk3Title != null) perk3Title.text = v ?? ""; });
        _viewModel.Perk3Desc.Subscribe(v  => { if (perk3Exp   != null) perk3Exp.text   = v ?? ""; });

        _viewModel.Perk1Raw.Subscribe(raw => SetPerkImage(perk1Img, raw));
        _viewModel.Perk2Raw.Subscribe(raw => SetPerkImage(perk2Img, raw));
        _viewModel.Perk3Raw.Subscribe(raw => SetPerkImage(perk3Img, raw));

        _viewModel.HandElementalName.Subscribe(name =>
        {
            if (handImg == null || string.IsNullOrEmpty(name)) return;
            var hand = HandInfoProvider.FromString(name);
            if (hand != HandElementalType.NONE)
                handImg.sprite = Resources.Load<Sprite>(HandInfoProvider.GetImagePath(hand));
        });

        _viewModel.BeforeInfo.Subscribe(v       => { if (beforeInfo  != null) beforeInfo.text  = v ?? ""; });
        _viewModel.AfterInfo.Subscribe(v        => { if (afterInfo   != null) afterInfo.text   = v ?? ""; });
        _viewModel.UpgradeCostLabel.Subscribe(v => { if (upgradeCost != null) upgradeCost.text = v ?? ""; });

        _viewModel.CanSelect.Subscribe(can =>
        {
            selectBtn1?.SetEnabled(can);
            selectBtn2?.SetEnabled(can);
            selectBtn3?.SetEnabled(can);
        });

        _viewModel.CanUpgrade.Subscribe(can => upgradeBtn?.SetEnabled(can));

        selectBtn1.clicked += () => _viewModel.OnSelectPerk(1);
        selectBtn2.clicked += () => _viewModel.OnSelectPerk(2);
        selectBtn3.clicked += () => _viewModel.OnSelectPerk(3);
        upgradeBtn.clicked += () => _viewModel.OnUpgrade();
    }

    private void SetPerkImage(Image img, string raw)
    {
        if (img == null || string.IsNullOrEmpty(raw)) return;
        img.sprite = Resources.Load<Sprite>($"Perks/{raw}");
    }

    private void OnDisable()
    {
        _viewModel?.Dispose();
        _viewModel = null;
    }
}