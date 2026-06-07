using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Threading.Tasks;

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
    private HashSet<string> _shownItems = new HashSet<string>();
    private bool _isShowing = false;

    private void OnEnable()
    {
        _shownItems.Clear();
        _queue.Clear();
        _isShowing = false;
        if (!TryCacheElements())
        {
            Debug.LogError("[ItemView] UI Toolkit root is not ready.");
            return;
        }
        _itemRepo ??= RepositoryFactory.Instance.Get<IItemRepository>();
        var panel = _root.Q<VisualElement>("Item");
        if (panel != null)
        {
            panel.style.transitionProperty = StyleKeyword.Null;
            panel.style.opacity   = 0f;
            panel.style.translate = new StyleTranslate(new Translate(0, 60, 0));
        }
    }

    public void ShowItem(string itemCode)
    {
        Debug.Log("itemCode : "  + itemCode);
        if (!TryCacheElements())
        {
            Debug.LogError("[ItemView] UI elements are missing. ShowItem aborted.");
            return;
        }

        if (_queue.Contains(itemCode) || _shownItems.Contains(itemCode)) return;

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
            _shownItems.Add(itemCode);

            if (!Enum.TryParse<ItemType>(itemCode, out var itemType))
            {
                Debug.LogError($"[ItemView] Unknown item code: {itemCode}");
                continue;
            }

            var panel = _root.Q<VisualElement>("Item");

            // 먼저 숨기고
            panel.style.transitionProperty = StyleKeyword.Null;
            panel.style.opacity   = 0f;
            panel.style.translate = new StyleTranslate(new Translate(0, 60, 0));

            yield return null; // 숨김 상태 적용 대기

            // 그 다음 데이터 세팅
            _itemTitle.text = ItemInfoProvider.GetDisplayName(itemType);
            _itemInfo.text  = ItemInfoProvider.GetDescription(itemType);
            var sprite = Resources.Load<Sprite>($"Items/{itemType}");
            if (sprite != null) _itemImg.sprite = sprite;

            // 그 다음 애니메이션
            yield return StartCoroutine(AnimateItem(panel));

            yield return new WaitForSeconds(3f);
            //yield return new WaitForSeconds(GameSetting.DELAY_MAP[SceneDataBridge.playerCamera] / 1000f);
            Debug.Log("[ItemView] SendPutAck 호출: " + itemCode);
            _ = SendPutAck(); 
        }
        _isShowing = false;
        _shownItems.Clear();  // ← 추가
        GetComponent<UIDocument>().enabled = false;
    }

    private IEnumerator AnimateItem(VisualElement panel)
    {
        if (panel == null) yield break;

        // 초기 숨김 코드 제거 (ProcessQueue에서 이미 처리)

        panel.style.transitionProperty = new List<StylePropertyName> { "opacity", "translate" };
        panel.style.transitionDuration = new List<TimeValue>
            { new TimeValue(0.4f, TimeUnit.Second), new TimeValue(0.4f, TimeUnit.Second) };
        panel.style.transitionTimingFunction = new List<EasingFunction>
            { new EasingFunction(EasingMode.EaseOutBack) };

        yield return null;

        panel.style.opacity   = 1f;
        panel.style.translate = new StyleTranslate(new Translate(0, 0, 0));

        yield return new WaitForSeconds(0.5f);
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
    
    // ItemView에 메서드 추가
    private async Task SendPutAck()
    {
        var ackResult = await _itemRepo.PutAck(SceneDataBridge.playerId);
        if (!ackResult.isSuccess)
            Debug.LogError($"[ItemView] PutAck failed: {ackResult.error?.code} {ackResult.error?.message}");
    }
}