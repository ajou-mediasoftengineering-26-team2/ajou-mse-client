using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

//202322158 이준상

/// <summary>
/// UI view class responsible for playing sequential text animations when introducing a game station.
/// Automatically handles UI Toolkit layout constraints and triggers the match start event upon completion.
/// </summary>
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

    /// <summary>
    /// Unity lifecycle method triggered when the object becomes active.
    /// Initializes the UIDocument reference, resets layout tracking flags, and caches UI components.
    /// </summary>
    private void OnEnable()
    {
        Debug.Log("[IntroduceStationView] OnEnable");
        uiDocument = GetComponent<UIDocument>();
        EnsureRoot();
        ResetLayoutState();
        CacheElements(force: true);
    }

    /// <summary>
    /// Public entry point to trigger the station introduction screen sequence.
    /// Refreshes the UIDocument context, injects station/event texts, and schedules the sequential animation.
    /// </summary>
    /// <param name="station">The name of the current station to display.</param>
    /// <param name="evtTitle">The title text of the triggered event.</param>
    /// <param name="evtDescription">The descriptive summary text of the event.</param>
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

    /// <summary>
    /// Flushes running UI schedules and unregisters panel event callbacks to ensure a clean state machine reset.
    /// </summary>
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

    /// <summary>
    /// Core execution logic for beginning the fade-in sequence. 
    /// Postpones execution if the layout bounds or panel contexts are not fully computed by the engine.
    /// </summary>
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

        InitInitialState(first, "first");
        InitInitialState(second, "second");
        InitInitialState(third, "third");
        InitInitialState(fourth, "fourth");
        InitInitialState(fifth, "fifth");
        
        Debug.Log("[IntroduceStationView] Scheduling sequence animation.");
        root.schedule.Execute(PlaySequenceAnimation).StartingIn(100);
    }

    /// <summary>
    /// Sets up a fallback ticking schedule to continuously check for geometry initialization in case events do not fire.
    /// </summary>
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

    /// <summary>
    /// Callback triggered when the visual element is attached to a live UI Toolkit panel context.
    /// </summary>
    /// <param name="evt">The attach event arguments payload.</param>
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

    /// <summary>
    /// Subscribes to geometry recalculation events to capture exact layout bounds once processed by the engine.
    /// </summary>
    private void RegisterGeometryWait()
    {
        if (root == null || waitingForGeometry) return;
        waitingForGeometry = true;
        root.UnregisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        root.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
    }

    /// <summary>
    /// Callback triggered when geometry bounds (width, height, position) change on the root canvas.
    /// </summary>
    /// <param name="evt">The geometry change event arguments payload.</param>
    private void OnGeometryChanged(GeometryChangedEvent evt)
    {
        if (!pendingStart) return;
        if (!IsLayoutReady()) return;
        waitingForGeometry = false;
        root.UnregisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        Debug.Log("[IntroduceStationView] Geometry ready. Attempting sequence animation.");
        TryStartAnimation();
    }

    /// <summary>
    /// Validates if the root element's dimensions have been successfully calculated and are greater than zero.
    /// </summary>
    /// <returns>True if dimensions are valid numbers and layout computation is complete; otherwise, false.</returns>
    private bool IsLayoutReady()
    {
        if (root == null || root.panel == null) return false;
        bool rootValid = !float.IsNaN(root.worldBound.width)
            && !float.IsNaN(root.worldBound.height)
            && root.worldBound.width > 0f
            && root.worldBound.height > 0f;
        return rootValid;
    }

    /// <summary>
    /// Safe internal wrapper to ensure the rootVisualElement reference is resolved from the active UIDocument.
    /// </summary>
    private void EnsureRoot()
    {
        if (uiDocument == null) uiDocument = GetComponent<UIDocument>();
        if (uiDocument != null)
        {
            root = uiDocument.rootVisualElement;
        }
    }

    /// <summary>
    /// Queries and caches structural VisualElements and UXML text labels via style classes and entity IDs.
    /// </summary>
    /// <param name="force">If true, flushes previously cached references to force a hard re-query.</param>
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

    /// <summary>
    /// Sets an item's startup opacity and position offset, then schedules standard UI transition rules.
    /// </summary>
    /// <param name="element">The target UI visual component.</param>
    /// <param name="name">Debug identifier name assigned to the component.</param>
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

    /// <summary>
    /// Coordinates the multi-tiered cascading delays that reveal the 5 distinct title and description text fields line-by-line.
    /// </summary>
    private void PlaySequenceAnimation()
    {
        if (first == null || second == null || third == null || fourth == null || fifth == null)
        {
            Debug.LogWarning("[IntroduceStationView] PlaySequenceAnimation missing labels.");
            return;
        }
        Debug.Log("[IntroduceStationView] PlaySequenceAnimation starting for 5 lines.");
        first.schedule.Execute(() => ShowElement(first)).StartingIn(0);
        second.schedule.Execute(() => ShowElement(second)).StartingIn(LineDelayMs);
        third.schedule.Execute(() => ShowElement(third)).StartingIn(LineDelayMs * 2);

        fourth.schedule.Execute(() => ShowElement(fourth)).StartingIn(LineDelayMs * 3);
        fifth.schedule.Execute(() => ShowElement(fifth)).StartingIn(LineDelayMs * 4);

        (fadeTarget ?? root).schedule.Execute(FadeOutAnimation)
            .StartingIn((LineDelayMs * 4) + FadeOutDelayMs);
    }

    /// <summary>
    /// Interpolates an element's visibility parameters to smoothly fade it into the visible view stack.
    /// </summary>
    /// <param name="element">The visual UI asset targeted for interpolation.</param>
    private void ShowElement(VisualElement element)
    {
        if (element == null) return;
        element.style.opacity = 1f;
        element.style.translate = new StyleTranslate(new Translate(0, 0, 0));
    }

    /// <summary>
    /// Initiates a global canvas fade-out and multi-threads a MatchStartEvent message through the architecture EventBus framework.
    /// </summary>
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