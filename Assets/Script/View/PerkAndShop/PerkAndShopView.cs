using System;
using UnityEngine;
using UnityEngine.UIElements;

// 202422170 주형준
public class PerkAndShopView : MonoBehaviour
{
    private PerkAndShopViewModel _viewModel;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        
        var selectBtn1 = root.Q<Button>("SelectBtn1");
        var selectBtn2 = root.Q<Button>("SelectBtn2");
        var selectBtn3 = root.Q<Button>("SelectBtn3");
        
        var perk1Title = root.Q<Label>("Perk1Title");
        var perk1Exp   = root.Q<Label>("Perk1Exp");
        var perk2Title = root.Q<Label>("Perk2Title");
        var perk2Exp   = root.Q<Label>("Perk2Exp");
        var perk3Title = root.Q<Label>("Perk3Title");
        var perk3Exp   = root.Q<Label>("Perk3Exp");
        
        var perk1Img = root.Q<Image>("Perk1Img");
        var perk2Img = root.Q<Image>("Perk2Img");
        var perk3Img = root.Q<Image>("Perk3Img");

        // 업그레이드 버튼
        var upgradeBtn = root.Q<Button>("UpgradeBtn");

        _viewModel = new PerkAndShopViewModel();
        _viewModel.SetPlayerInfo(SceneDataBridge.playerId, SceneDataBridge.MatchId);
        _viewModel.Initialize();


        root.style.display = DisplayStyle.None;
        _viewModel.IsVisible.Subscribe(visible =>
            root.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None);
        
        _viewModel.Perk1Title.Subscribe(v => perk1Title.text = v ?? "");
        _viewModel.Perk1Desc.Subscribe(v  => perk1Exp.text   = v ?? "");
        _viewModel.Perk2Title.Subscribe(v => perk2Title.text = v ?? "");
        _viewModel.Perk2Desc.Subscribe(v  => perk2Exp.text   = v ?? "");
        _viewModel.Perk3Title.Subscribe(v => perk3Title.text = v ?? "");
        _viewModel.Perk3Desc.Subscribe(v  => perk3Exp.text   = v ?? "");
        
        _viewModel.Perk1Raw.Subscribe(raw => SetPerkImage(perk1Img, raw));
        _viewModel.Perk2Raw.Subscribe(raw => SetPerkImage(perk2Img, raw));
        _viewModel.Perk3Raw.Subscribe(raw => SetPerkImage(perk3Img, raw));

        _viewModel.CanSelect.Subscribe(can =>
        {
            selectBtn1.SetEnabled(can);
            selectBtn2.SetEnabled(can);
            selectBtn3.SetEnabled(can);
        });

        upgradeBtn.SetEnabled(false);
        
        selectBtn1.clicked += () => _viewModel.OnSelectPerk(1);
        selectBtn2.clicked += () => _viewModel.OnSelectPerk(2);
        selectBtn3.clicked += () => _viewModel.OnSelectPerk(3);
    }

    private void SetPerkImage(Image imgElement, string raw)
    {
        if (string.IsNullOrEmpty(raw)) return;
        if (!Enum.TryParse<PerkType>(raw, out var perkType)) return;
        imgElement.sprite = Resources.Load<Sprite>($"Perks/{perkType}");
    }

    private void OnDestroy() => _viewModel?.Dispose();
}