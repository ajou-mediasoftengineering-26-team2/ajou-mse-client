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

    public void StartAnimation(string station = "Seoul")
    {
        Debug.Log($"[IntroduceStationView] StartAnimation called station={station}");
        EnsureRoot();
        ResetLayoutState();
        CacheElements(force: true);
        
        Debug.Log($"[IntroduceStationView] root={(root != null)} panel={(root != null && root.panel != null)} container={(container != null)} first={(first != null)} second={(second != null)} third={(third != null)}");
        if (root == null || second == null)
        {
            Debug.LogWarning("[IntroduceStationView] Missing root/second/container. Abort animation.");
            return;
        }
        
        pendingStation = station;
        second.text = station;
        
        // 1. 전체 컨테이너와 루트를 먼저 활성화 및 투명도 초기화
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
            fadeTarget.style.opacity = 1f; // 페이드아웃 되었을 수 있으니 되돌림
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

        // 2. 글자들의 최초 상태(숨김, 아래로 내려감)를 강제로 적용
        InitInitialState(first, "first");
        InitInitialState(second, "second");
        InitInitialState(third, "third");
        
        // 3. [핵심] 유니티 UI Toolkit이 초기화된 스타일(Opacity 0)을 인지할 시간을 줌 (약 50~100ms 뒤 재생)
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
        Debug.Log($"[IntroduceStationView] Layout check rootBound={root.worldBound} containerBound={(container != null ? container.worldBound.ToString() : "null")}");
        return rootValid;
    }

    private void EnsureRoot()
    {
        if (uiDocument == null) uiDocument = GetComponent<UIDocument>();
        if (uiDocument != null)
        {
            root = uiDocument.rootVisualElement;
        }
        Debug.Log($"[IntroduceStationView] EnsureRoot uiDocument={(uiDocument != null)} root={(root != null)}");
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
        }

        screenRoot ??= root.Q<VisualElement>(className: "station-screen-root");
        container ??= root.Q<VisualElement>(className: "station-container");
        fadeTarget ??= screenRoot ?? container ?? root;
        
        // UXML에 작성하신 이름(name 또는 class)에 맞게 가져오기
        // 만약 UXML에 name을 "first", "second", "third"로 하셨다면 아래대로 작동합니다.
        first ??= root.Q<Label>("first");
        second ??= root.Q<Label>("second");
        third ??= root.Q<Label>("third");
        Debug.Log($"[IntroduceStationView] CacheElements container={(container != null)} first={(first != null)} second={(second != null)} third={(third != null)}");
    }

    private void InitInitialState(VisualElement element, string name)
    {
        if (element == null)
        {
            Debug.LogWarning("[IntroduceStationView] InitInitialState called with null element.");
            return;
        }

        Debug.Log($"[IntroduceStationView] InitInitialState {name} display={element.resolvedStyle.display} opacity={element.resolvedStyle.opacity} translate={element.resolvedStyle.translate} worldBound={element.worldBound}");
        // 트랜지션 기능을 잠시 끄고 초기값 강제 대입 (이래야 툭 튀는 현상이 없음)
        element.style.transitionProperty = StyleKeyword.None; 
        element.style.opacity = 0f;
        element.style.translate = new StyleTranslate(new Translate(0, 20, 0));

        // 다음 프레임에 트랜지션이 먹도록 세팅
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
        if (first == null || second == null || third == null)
        {
            Debug.LogWarning("[IntroduceStationView] PlaySequenceAnimation missing labels.");
            return;
        }
        Debug.Log("[IntroduceStationView] PlaySequenceAnimation");

        // 첫 번째 줄 등장
        first.schedule.Execute(() => ShowElement(first)).StartingIn(0);

        // 두 번째 줄 등장
        second.schedule.Execute(() => ShowElement(second)).StartingIn(LineDelayMs);

        // 세 번째 줄 등장
        third.schedule.Execute(() => ShowElement(third)).StartingIn(LineDelayMs * 2);

        // 모든 글자가 다 뜨고 난 뒤(LineDelayMs * 2) + 대기 시간(FadeOutDelayMs) 후 페이드아웃
        (fadeTarget ?? root).schedule.Execute(FadeOutAnimation)
            .StartingIn((LineDelayMs * 2) + FadeOutDelayMs);
    }

    private void ShowElement(VisualElement element)
    {
        if (element == null) return;
        Debug.Log($"[IntroduceStationView] ShowElement name={element.name} display={element.resolvedStyle.display} beforeOpacity={element.resolvedStyle.opacity} worldBound={element.worldBound}");
        element.style.opacity = 1f;
        element.style.translate = new StyleTranslate(new Translate(0, 0, 0));
        Debug.Log($"[IntroduceStationView] ShowElement name={element.name} afterOpacity={element.resolvedStyle.opacity} worldBound={element.worldBound}");
    }

    private void FadeOutAnimation()
    {
        if (fadeTarget == null)
        {
            Debug.LogWarning("[IntroduceStationView] FadeOutAnimation target is null.");
            return;
        }
        Debug.Log("[IntroduceStationView] FadeOutAnimation");

        // 컨테이너 전체 페이드아웃 트랜지션 설정
        fadeTarget.style.transitionProperty = new List<StylePropertyName> { "opacity" };
        fadeTarget.style.transitionDuration = new List<TimeValue> { new TimeValue(FadeOutDurationSec, TimeUnit.Second) };
        fadeTarget.style.opacity = 0f;

        // 페이드아웃 애니메이션 완료 후 오브젝트/UI 비활성화
        fadeTarget.schedule.Execute(() =>
        {
            if (uiDocument != null)
            {
                // UI 도큐먼트 컴포넌트 자체를 꺼서 화면에서 완전히 숨김
                uiDocument.enabled = false; 
            }
        }).StartingIn((int)(FadeOutDurationSec * 1000));
        EventBus.Publish(new MatchStartEvent());
    }
}