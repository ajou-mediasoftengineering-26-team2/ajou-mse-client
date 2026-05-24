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
        //업그레이드 샵 부분 전까지 임시
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
        
        _viewModel.CanSelect.Subscribe(can =>
        {
            selectBtn1.SetEnabled(can);
            selectBtn2.SetEnabled(can);
            selectBtn3.SetEnabled(can);
        });

        // 업그레이드 비활성
        upgradeBtn.SetEnabled(false);

        // 버튼 이벤트
        selectBtn1.clicked += () => _viewModel.OnSelectPerk(1);
        selectBtn2.clicked += () => _viewModel.OnSelectPerk(2);
        selectBtn3.clicked += () => _viewModel.OnSelectPerk(3);
    }

    private void OnDestroy() => _viewModel?.Dispose();
}