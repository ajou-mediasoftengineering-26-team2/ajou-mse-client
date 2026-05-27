using System;
using UnityEngine;
using UnityEngine.UIElements;

// 202422170 주형준
public class ItemView : MonoBehaviour
{
    private Image _itemImg;
    private Label _itemTitle;
    private Label _itemInfo;

    private void OnEnable()
    {
        var root   = GetComponent<UIDocument>().rootVisualElement;
        _itemImg   = root.Q<Image>("ItemImg");
        _itemTitle = root.Q<Label>("ItemTitle");
        _itemInfo  = root.Q<Label>("ItemInfo");
    }

    public void ShowItem(string itemCode)
    {
        if (!Enum.TryParse<ItemType>(itemCode, out var itemType)) return;
        _itemTitle.text = ItemInfoProvider.GetDisplayName(itemType);
        _itemInfo.text  = ItemInfoProvider.GetDescription(itemType);
        var sprite = Resources.Load<Sprite>($"Items/{itemType}");
        if (sprite != null) _itemImg.sprite = sprite;
    }
}