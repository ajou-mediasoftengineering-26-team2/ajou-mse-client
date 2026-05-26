using System;
using UnityEngine;
using UnityEngine.UIElements;

// 202422170 주형준
public class ItemView : MonoBehaviour
{
    [SerializeField] private UIDocument battleUI;

    private ItemViewModel _viewModel;

    private void OnEnable()
    {
        var popupRoot  = GetComponent<UIDocument>().rootVisualElement;
        var itemImg    = popupRoot.Q<Image>("ItemImg");
        var itemTitle  = popupRoot.Q<Label>("ItemTitle");
        var itemInfo   = popupRoot.Q<Label>("ItemInfo");

        var battleRoot = battleUI.rootVisualElement;
        var slot1 = battleRoot.Q<VisualElement>("Item1");
        var slot2 = battleRoot.Q<VisualElement>("Item2");
        var slot3 = battleRoot.Q<VisualElement>("Item3");

        _viewModel = new ItemViewModel();
        _viewModel.SetPlayerInfo(SceneDataBridge.playerId, SceneDataBridge.MatchId);
        _viewModel.Initialize();

        popupRoot.style.display = DisplayStyle.None;
        _viewModel.IsVisible.Subscribe(visible =>
        {
            popupRoot.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
            if (visible)
                EventBus.Publish(new ItemAnimationEndEvent()); // 임시 애니메이션
        });

        _viewModel.ItemRaw.Subscribe(raw  => SetItemImage(itemImg, raw));
        _viewModel.ItemName.Subscribe(v   => itemTitle.text = v ?? "");
        _viewModel.ItemDesc.Subscribe(v   => itemInfo.text  = v ?? "");

        _viewModel.Item1Raw.Subscribe(raw => SetSlotImage(slot1, raw));
        _viewModel.Item2Raw.Subscribe(raw => SetSlotImage(slot2, raw));
        _viewModel.Item3Raw.Subscribe(raw => SetSlotImage(slot3, raw));
    }

    private void SetItemImage(Image img, string raw)
    {
        if (string.IsNullOrEmpty(raw) || img == null) return;
        if (!Enum.TryParse<ItemType>(raw, out var itemType)) return;
        img.sprite = Resources.Load<Sprite>($"Items/{itemType}");
    }

    private void SetSlotImage(VisualElement slot, string raw)
    {
        if (slot == null) return;
        if (string.IsNullOrEmpty(raw)) { slot.style.backgroundImage = StyleKeyword.None; return; }
        if (!Enum.TryParse<ItemType>(raw, out var itemType)) return;
        var sprite = Resources.Load<Sprite>($"Items/{itemType}");
        if (sprite != null)
            slot.style.backgroundImage = new StyleBackground(sprite);
    }

    private void OnDisable() => _viewModel?.Dispose();
}