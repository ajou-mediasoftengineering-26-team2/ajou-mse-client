using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class Toast : MonoBehaviour
{
    private static VisualElement _root;
    private static VisualElement _toastLayer;
    
    void OnEnable()
    {
        TryBindRoot(GetComponent<UIDocument>());
    }

    public static void Show(string message, float duration = 2.0f)
    {
        if (string.IsNullOrEmpty(message)) return;
        EnsureRoot();
        if (_root == null) return;
        EnsureToastLayer();

        Label toast = new Label(message);
        toast.style.unityTextAlign = TextAnchor.MiddleCenter;
        toast.style.backgroundColor = new Color(0, 0, 0, 0.7f); // 반투명 검정
        toast.style.color = Color.white;
        toast.style.paddingLeft = 20;
        toast.style.paddingRight = 20;
        toast.style.paddingTop = 10;
        toast.style.paddingBottom = 10;
        toast.style.borderTopLeftRadius = 15;
        toast.style.borderTopRightRadius = 15;
        toast.style.borderBottomLeftRadius = 15;
        toast.style.borderBottomRightRadius = 15;
        toast.style.marginTop = 6;
        toast.style.marginBottom = -30;
        toast.pickingMode = PickingMode.Ignore;

        toast.style.opacity = 0;
        toast.style.transitionProperty = new List<StylePropertyName> { "opacity", "margin-bottom" };
        toast.style.transitionDuration = new List<TimeValue>
        {
            new TimeValue(0.35f, TimeUnit.Second),
            new TimeValue(0.35f, TimeUnit.Second)
        };

        _toastLayer.Add(toast);
        VisualElement currentLayer = _toastLayer;

        toast.schedule.Execute(() =>
        {
            toast.style.opacity = 1;
            toast.style.marginBottom = 6;
        }).StartingIn(500);

        toast.schedule.Execute(() => {
            toast.style.opacity = 0;
            toast.style.marginBottom = -30;
            toast.schedule.Execute(() => currentLayer?.Remove(toast)).StartingIn(350);
        }).StartingIn((int)(duration * 1000));
    }

    private static void EnsureRoot()
    {
        if (_root != null && _root.panel != null) return;

        _root = null;
        _toastLayer = null;

        UIDocument[] documents = Object.FindObjectsOfType<UIDocument>();
        UIDocument best = null;
        UIDocument fallback = null;

        foreach (UIDocument document in documents)
        {
            if (document == null || !document.isActiveAndEnabled) continue;
            if (document.rootVisualElement == null) continue;
            fallback ??= document;
            if (!IsUsableDocument(document)) continue;

            if (best == null || document.sortingOrder > best.sortingOrder)
            {
                best = document;
            }
        }

        TryBindRoot(best ?? fallback);
    }

    private static void TryBindRoot(UIDocument document)
    {
        if (!IsUsableDocument(document)) return;

        _root = document.rootVisualElement;
        _toastLayer = null;
    }

    private static bool IsUsableDocument(UIDocument document)
    {
        if (document == null || !document.isActiveAndEnabled) return false;
        VisualElement root = document.rootVisualElement;
        if (root == null) return false;

        
        return root.childCount > 0;
    }

    private static void EnsureToastLayer()
    {
        if (_toastLayer != null && _toastLayer.panel != null) return;

        _toastLayer = new VisualElement();
        _toastLayer.name = "ToastLayer";
        _toastLayer.style.position = Position.Absolute;
        _toastLayer.style.left = 0;
        _toastLayer.style.right = 0;
        _toastLayer.style.top = 0;
        _toastLayer.style.bottom = 0;
        _toastLayer.style.justifyContent = Justify.FlexEnd;
        _toastLayer.style.alignItems = Align.Center;
        _toastLayer.style.paddingBottom = 50;
        _toastLayer.pickingMode = PickingMode.Ignore;

        _root.Add(_toastLayer);
    }
}
