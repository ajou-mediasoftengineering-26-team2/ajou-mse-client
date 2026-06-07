using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class IntroduceStationView : MonoBehaviour
{
    private UIDocument uiDocument;
    private VisualElement root;
    private VisualElement screenRoot;
    private VisualElement container;
    private VisualElement fadeTarget;
    private Label first;
    private Label second;
    private Label third;
    private Label fourth;
    private Label fifth;
    private bool pendingStart;
    private string pendingStation = "Seoul";
    private bool waitingForGeometry;
    private IVisualElementScheduledItem layoutRetryItem;

    private const int LineDelayMs = 1000;
    private const int FadeOutDelayMs = 2500;
    private const float FadeOutDurationSec = 0.5f;
    private const int LayoutRetryMs = 50;

    private void OnEnable()
    {
        Debug.Log("[IntroduceStationView] OnEnable");
        uiDocument = GetComponent<UIDocument>();
        EnsureRoot();
        ResetLayoutState();
        CacheElements(force: true);
    }

    public void StartAnimation(string station, string evtTitle, string evtDescription)
    {
        if (uiDocument == null) uiDocument = GetComponent<UIDocument>();
        if (uiDocument != null)
        {
            uiDocument.enabled = false; 
            uiDocument.enabled = true;  
            root = uiDocument.rootVisualElement;
        }
        
        Debug.Log($"[IntroduceStationView] StartAnimation called station={station}");
        EnsureRoot();
        ResetLayoutState();
        CacheElements(force: true);
        
        if (root == null || second == null)
        {
            Debug.LogWarning("[IntroduceStationView] Missing root/second/container. Abort animation.");
            return;
        }
        
        pendingStation = station;
        second.text = station;
        fourth.text = evtTitle;
        fifth.text = evtDescription;
        
        root.style.display = DisplayStyle.Flex;
        root.style.opacity = 1f;
        if (screenRoot != null)
        {
            screenRoot.style.display = DisplayStyle.Flex;
            screenRoot.style.width = Length.Percent(100);
            screenRoot.style.height = Length.Percent(100);
        }
        if (container != null)
        {
            container.style.display = DisplayStyle.Flex;
            container.style.width = Length.Percent(100);
            container.style.height = Length.Percent(100);
            container.style.flexGrow = 1f;
            container.style.alignSelf = Align.Stretch;
        }
        if (fadeTarget != null)
        {
            fadeTarget.style.opacity = 1f; 
        }

        pendingStart = true;
        TryStartAnimation();
    }

    private void ResetLayoutState()
    {
        pendingStart = false;
        waitingForGeometry = false;
        if (layoutRetryItem != null)
        {
            layoutRetryItem.Pause();
            layoutRetryItem = null;
        }
        if (root != null)
        {
            root.UnregisterCallback<AttachToPanelEvent>(OnAttachedToPanel);
            root.UnregisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }
    }

    private void TryStartAnimation()
    {
        if (!pendingStart || root == null) return;

        if (root.panel == null)
        {
            Debug.Log("[IntroduceStationView] root.panel is null. Waiting AttachToPanelEvent.");
            root.UnregisterCallback<AttachToPanelEvent>(OnAttachedToPanel);
            root.RegisterCallback<AttachToPanelEvent>(OnAttachedToPanel);
            return;
        }

        if (!IsLayoutReady())
        {
            Debug.Log("[IntroduceStationView] Layout not ready. Waiting GeometryChangedEvent.");
            RegisterGeometryWait();
            ScheduleLayoutRetry();
            return;
        }

        pendingStart = false;
        waitingForGeometry = false;

        // [수정] 4번째, 5번째 글자도 최초 상태(숨김, 아래로 내려감) 강제 적용
        InitInitialState(first, "first");
        InitInitialState(second, "second");
        InitInitialState(third, "third");
        InitInitialState(fourth, "fourth");
        InitInitialState(fifth, "fifth");
        
        Debug.Log("[IntroduceStationView] Scheduling sequence animation.");
        root.schedule.Execute(PlaySequenceAnimation).StartingIn(100);
    }

    private void ScheduleLayoutRetry()
    {
        if (layoutRetryItem != null || root == null) return;
        layoutRetryItem = root.schedule.Execute(() =>
        {
            if (!pendingStart || root == null)
            {
                layoutRetryItem?.Pause();
                layoutRetryItem = null;
                return;
            }

            if (root.panel == null || !IsLayoutReady()) return;

            layoutRetryItem?.Pause();
            layoutRetryItem = null;
            TryStartAnimation();
        }).Every(LayoutRetryMs);
    }

    private void OnAttachedToPanel(AttachToPanelEvent evt)
    {
        Debug.Log("[IntroduceStationView] OnAttachedToPanel");
        if (root != null)
        {
            root.UnregisterCallback<AttachToPanelEvent>(OnAttachedToPanel);
        }
        if (!pendingStart) return;

        CacheElements(force: true);
        if (second != null)
        {
            second.text = pendingStation;
        }

        TryStartAnimation();
    }

    private void RegisterGeometryWait()
    {
        if (root == null || waitingForGeometry) return;
        waitingForGeometry = true;
        root.UnregisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        root.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
    }

    private void OnGeometryChanged(GeometryChangedEvent evt)
    {
        if (!pendingStart) return;
        if (!IsLayoutReady()) return;
        waitingForGeometry = false;
        root.UnregisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        Debug.Log("[IntroduceStationView] Geometry ready. Attempting sequence animation.");
        TryStartAnimation();
    }

    private bool IsLayoutReady()
    {
        if (root == null || root.panel == null) return false;
        bool rootValid = !float.IsNaN(root.worldBound.width)
            && !float.IsNaN(root.worldBound.height)
            && root.worldBound.width > 0f
            && root.worldBound.height > 0f;
        return rootValid;
    }

    private void EnsureRoot()
    {
        if (uiDocument == null) uiDocument = GetComponent<UIDocument>();
        if (uiDocument != null)
        {
            root = uiDocument.rootVisualElement;
        }
    }

    private void CacheElements(bool force = false)
    {
        if (root == null) return;

        if (force)
        {
            screenRoot = null;
            container = null;
            fadeTarget = null;
            first = null;
            second = null;
            third = null;
            fourth = null;
            fifth = null;
        }

        screenRoot ??= root.Q<VisualElement>(className: "station-screen-root");
        container ??= root.Q<VisualElement>(className: "station-container");
        fadeTarget ??= screenRoot ?? container ?? root;
        
        first ??= root.Q<Label>("first");
        second ??= root.Q<Label>("second");
        third ??= root.Q<Label>("third");
        fourth ??= root.Q<Label>("fourth");
        fifth ??= root.Q<Label>("fifth");
    }

    private void InitInitialState(VisualElement element, string name)
    {
        if (element == null)
        {
            Debug.LogWarning($"[IntroduceStationView] InitInitialState failed: {name} is null.");
            return;
        }

        element.style.transitionProperty = StyleKeyword.None; 
        element.style.opacity = 0f;
        element.style.translate = new StyleTranslate(new Translate(0, 20, 0));

        root.schedule.Execute(() =>
        {
            element.style.transitionProperty = new List<StylePropertyName> { "opacity", "translate" };
            element.style.transitionDuration = new List<TimeValue>
            {
                new TimeValue(0.35f, TimeUnit.Second),
                new TimeValue(0.35f, TimeUnit.Second)
            };
            element.style.transitionTimingFunction = new List<EasingFunction>
            {
                new EasingFunction(EasingMode.EaseOutBack)
            };
        }).StartingIn(10);
    }

    private void PlaySequenceAnimation()
    {
        // [수정] 4번째, 5번째 라벨 검증 추가
        if (first == null || second == null || third == null || fourth == null || fifth == null)
        {
            Debug.LogWarning("[IntroduceStationView] PlaySequenceAnimation missing labels.");
            return;
        }
        Debug.Log("[IntroduceStationView] PlaySequenceAnimation starting for 5 lines.");

        // 1~3번째 줄 등장
        first.schedule.Execute(() => ShowElement(first)).StartingIn(0);
        second.schedule.Execute(() => ShowElement(second)).StartingIn(LineDelayMs);
        third.schedule.Execute(() => ShowElement(third)).StartingIn(LineDelayMs * 2);

        // [추가] 4~5번째 줄 등장 시퀀스 확장
        fourth.schedule.Execute(() => ShowElement(fourth)).StartingIn(LineDelayMs * 3);
        fifth.schedule.Execute(() => ShowElement(fifth)).StartingIn(LineDelayMs * 4);

        // [수정] 모든 글자가 다 뜨고 난 뒤(LineDelayMs * 4) + 대기 시간(FadeOutDelayMs) 후 페이드아웃
        (fadeTarget ?? root).schedule.Execute(FadeOutAnimation)
            .StartingIn((LineDelayMs * 4) + FadeOutDelayMs);
    }

    private void ShowElement(VisualElement element)
    {
        if (element == null) return;
        element.style.opacity = 1f;
        element.style.translate = new StyleTranslate(new Translate(0, 0, 0));
    }

    private void FadeOutAnimation()
    {
        if (fadeTarget == null)
        {
            Debug.LogWarning("[IntroduceStationView] FadeOutAnimation target is null.");
            return;
        }
        Debug.Log("[IntroduceStationView] FadeOutAnimation");

        fadeTarget.style.transitionProperty = new List<StylePropertyName> { "opacity" };
        fadeTarget.style.transitionDuration = new List<TimeValue> { new TimeValue(FadeOutDurationSec, TimeUnit.Second) };
        fadeTarget.style.opacity = 0f;

        fadeTarget.schedule.Execute(() =>
        {
            if (uiDocument != null)
            {
                uiDocument.enabled = false; 
            }
        }).StartingIn((int)(FadeOutDurationSec * 1000));
        
        EventBus.Publish(new MatchStartEvent());
    }

    
}