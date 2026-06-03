using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// 202422170 주형준
public class ItemView : MonoBehaviour
{
    private UIDocument _uiDocument;
    private VisualElement _root;
    private Image _itemImg;
    private Label _itemTitle;
    private Label _itemInfo;
    private IItemRepository _itemRepo;

    private Queue<string> _queue = new Queue<string>();
    private bool _isShowing = false;

    private void OnEnable()
    {
        if (!TryCacheElements())
        {
            Debug.LogError("[ItemView] UI Toolkit root is not ready.");
            return;
        }
        _itemRepo ??= RepositoryFactory.Instance.Get<IItemRepository>();
    }

    public void ShowItem(string itemCode)
    {
        if (!TryCacheElements())
        {
            Debug.LogError("[ItemView] UI elements are missing. ShowItem aborted.");
            return;
        }

        _queue.Enqueue(itemCode);
        if (!_isShowing)
            StartCoroutine(ProcessQueue());
    }

    private IEnumerator ProcessQueue()
    {
        _isShowing = true;
        while (_queue.Count > 0)
        {
            string itemCode = _queue.Dequeue();
            if (!Enum.TryParse<ItemType>(itemCode, out var itemType))
            {
                Debug.LogError($"[ItemView] Unknown item code: {itemCode}");
                continue;
            }
            Debug.Log("item type" + itemType);
            _itemTitle.text = ItemInfoProvider.GetDisplayName(itemType);
            _itemInfo.text  = ItemInfoProvider.GetDescription(itemType);
            var sprite = Resources.Load<Sprite>($"Items/{itemType}");
            if (sprite != null) _itemImg.sprite = sprite;

            yield return new WaitForSeconds(3f);
            yield return new WaitForSeconds(GameSetting.DELAY_MAP[SceneDataBridge.playerCamera] / 1000f);
            _ = _itemRepo.PutAck(SceneDataBridge.playerId);
        }
        _isShowing = false;
        GetComponent<UIDocument>().enabled = false;
    }

    private bool TryCacheElements()
    {
        _uiDocument ??= GetComponent<UIDocument>();
        if (_uiDocument == null) return false;

        var currentRoot = _uiDocument.rootVisualElement;
        if (currentRoot == null) return false;

        if (!ReferenceEquals(currentRoot, _root) || _itemImg == null || _itemTitle == null || _itemInfo == null)
        {
            _root      = currentRoot;
            _itemImg   = _root.Q<Image>("ItemImg");
            _itemTitle = _root.Q<Label>("ItemTitle");
            _itemInfo  = _root.Q<Label>("ItemInfo");
        }

        return _itemImg != null && _itemTitle != null && _itemInfo != null;
    }
}