using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

//202322158 이준상

/// <summary>
/// UI class showing the hand action you and the other person chose.
/// All animation codes are produced by AI.
/// </summary>
public class ChoiceRevealView : MonoBehaviour
{
    private VisualElement _container;
    private VisualElement _leftPlayerGroup;
    private VisualElement _rightPlayerGroup;

    private VisualElement _leftChoiceImage;
    private VisualElement _rightChoiceImage;
    private Label _leftStatusLabel;
    private Label _rightStatusLabel;
    private Label _leftNameLabel;
    private Label _rightNameLabel;
    private Label _leftActionLabel;
    private Label _rightActionLabel;

    private Coroutine _animationCoroutine;

    /// <summary>
    /// Unity lifecycle method triggered when the object becomes enabled and active.
    /// Caches UI elements and snaps them to their initial visual states.
    /// </summary>
    void OnEnable()
    {
        TryCacheElements();
        SnapToInitialState();
    }

    /// <summary>
    /// Unity lifecycle method triggered when the object becomes disabled or inactive.
    /// Ensures running animation coroutines are safely stopped to prevent memory leaks.
    /// </summary>
    void OnDisable()
    {
        if (_animationCoroutine != null)
        {
            StopCoroutine(_animationCoroutine);
            _animationCoroutine = null;
        }
    }

    /// <summary>
    /// Public entry point for external testing. Validates UI initialization,
    /// configures player data, and triggers the choice reveal sequence.
    /// </summary>
    /// <param name="leftPlayer">The data model for the left player.</param>
    /// <param name="rightPlayer">The data model for the right player.</param>
    public void StartChoiceReveal(PlayerInfoModel leftPlayer, PlayerInfoModel rightPlayer)
    {
        if (!TryCacheElements())
        {
            Debug.LogError("[ChoiceReveal] UIDocument가 준비되지 않아 애니메이션을 시작할 수 없습니다.");
            return;
        }

        ShowContainer();
        SnapToInitialState();

        Sprite leftSprite = ConfigurePlayerUI(leftPlayer, isLeft: true);
        Sprite rightSprite = ConfigurePlayerUI(rightPlayer, isLeft: false);
        RevealChoices(leftSprite, rightSprite);
    }

    /// <summary>
    /// Instantly resets UI elements to their baseline hidden state by temporarily removing transitions.
    /// </summary>
    private void SnapToInitialState()
    {
        if (_container != null)
        {
            _container.style.transitionProperty = StyleKeyword.Null; // Temporarily remove transitions
            _container.style.opacity = 1f; // The background container remains visible
        }

        SnapElementZero(_leftPlayerGroup);
        SnapElementZero(_rightPlayerGroup);
        ResetBorders(_leftChoiceImage);
        ResetBorders(_rightChoiceImage);
    }

    /// <summary>
    /// Resets a specific visual element's opacity and position immediately without interpolation.
    /// </summary>
    /// <param name="element">The target VisualElement to reset.</param>
    private void SnapElementZero(VisualElement element)
    {
        if (element == null) return;
        element.style.transitionProperty = StyleKeyword.Null;
        element.style.opacity = 0f;
        element.style.translate = new StyleTranslate(new Translate(0, 50, 0));
    }

    /// <summary>
    /// Clears any dynamic border colors assigned to choice image container slots.
    /// </summary>
    /// <param name="image">The target card/choice image frame to reset.</param>
    private void ResetBorders(VisualElement image)
    {
        if (image == null) return;
        image.style.borderTopColor = StyleKeyword.Null;
        image.style.borderBottomColor = StyleKeyword.Null;
        image.style.borderLeftColor = StyleKeyword.Null;
        image.style.borderRightColor = StyleKeyword.Null;
    }

    /// <summary>
    /// Sets up background choices and handles thread-safe sequencing for the layout animations.
    /// </summary>
    /// <param name="leftPlayerSprite">The resolved card artwork for the left player.</param>
    /// <param name="rightPlayerSprite">The resolved card artwork for the right player.</param>
    public void RevealChoices(Sprite leftPlayerSprite, Sprite rightPlayerSprite)
    {
        if (!TryCacheElements())
        {
            Debug.LogError("[ChoiceReveal] UIDocument가 준비되지 않아 애니메이션을 시작할 수 없습니다.");
            return;
        }

        ShowContainer();

        if (_animationCoroutine != null)
        {
            StopCoroutine(_animationCoroutine);
        }

        SnapToInitialState();

        if (leftPlayerSprite != null && _leftChoiceImage != null)
            _leftChoiceImage.style.backgroundImage = new StyleBackground(leftPlayerSprite);

        if (rightPlayerSprite != null && _rightChoiceImage != null)
            _rightChoiceImage.style.backgroundImage = new StyleBackground(rightPlayerSprite);

        _animationCoroutine = StartCoroutine(PlaySequenceAnimationCoroutine());
    }

    /// <summary>
    /// Coroutine that executes the sequential visual timeline for revealing player actions,
    /// applying transitions, altering border colors, and fading out the view.
    /// </summary>
    private IEnumerator PlaySequenceAnimationCoroutine()
    {
        ApplyTransitionRules(_leftPlayerGroup);
        ApplyTransitionRules(_rightPlayerGroup);

        yield return null;

        yield return new WaitForSeconds(2.0f);
        if (_leftPlayerGroup != null)
        {
            _leftPlayerGroup.style.opacity = 1f;
            _leftPlayerGroup.style.translate = new StyleTranslate(new Translate(0, 0, 0));

            if (_leftChoiceImage != null)
            {
                Color color = new Color32(255, 200, 0, 255);
                _leftChoiceImage.style.borderTopColor = new StyleColor(color);
                _leftChoiceImage.style.borderBottomColor = new StyleColor(color);
                _leftChoiceImage.style.borderLeftColor = new StyleColor(color);
                _leftChoiceImage.style.borderRightColor = new StyleColor(color);
            }
        }

        yield return new WaitForSeconds(1.5f);
        if (_rightPlayerGroup != null)
        {
            _rightPlayerGroup.style.opacity = 1f;
            _rightPlayerGroup.style.translate = new StyleTranslate(new Translate(0, 0, 0));

            if (_rightChoiceImage != null)
            {
                Color orangeColor = new Color32(255, 100, 0, 255);
                _rightChoiceImage.style.borderTopColor = new StyleColor(orangeColor);
                _rightChoiceImage.style.borderBottomColor = new StyleColor(orangeColor);
                _rightChoiceImage.style.borderLeftColor = new StyleColor(orangeColor);
                _rightChoiceImage.style.borderRightColor = new StyleColor(orangeColor);
            }
        }

        yield return new WaitForSeconds(2.5f);
        if (_container != null)
        {
            _container.style.transitionProperty = new List<StylePropertyName> { "opacity" };
            _container.style.transitionDuration = new List<TimeValue> { new TimeValue(0.5f, TimeUnit.Second) };
            _container.style.opacity = 0f;
        }

        yield return new WaitForSeconds(0.5f);
        if (_container != null)
        {
            _container.style.display = DisplayStyle.None;
        }

        _animationCoroutine = null;
    }

    /// <summary>
    /// Configures UI Toolkit transition curves, durations, and easing functions for an element.
    /// </summary>
    /// <param name="element">The visual container to receive transition parameters.</param>
    private void ApplyTransitionRules(VisualElement element)
    {
        if (element == null) return;
        element.style.transitionProperty = new List<StylePropertyName> { "opacity", "translate" };
        element.style.transitionDuration = new List<TimeValue>
            { new TimeValue(0.5f, TimeUnit.Second), new TimeValue(0.5f, TimeUnit.Second) };
        element.style.transitionTimingFunction = new List<EasingFunction>
            { new EasingFunction(EasingMode.EaseOutBack) };
    }

    /// <summary>
    /// Attempts to query and cache all required VisualElements from the root UIDocument.
    /// </summary>
    /// <returns>True if the root container was successfully found and cached, otherwise false.</returns>
    private bool TryCacheElements()
    {
        var uiDoc = GetComponent<UIDocument>();
        if (uiDoc == null) return false;

        var root = uiDoc.rootVisualElement;
        if (root == null) return false;

        _container = root.Q<VisualElement>(className: "display-container");

        var playerGroups = root.Query<VisualElement>(className: "player-group").ToList();
        _leftPlayerGroup = playerGroups.Count > 0 ? playerGroups[0] : null;
        _rightPlayerGroup = playerGroups.Count > 1 ? playerGroups[1] : null;

        _leftChoiceImage = root.Q<VisualElement>("left-choice-image");
        _rightChoiceImage = root.Q<VisualElement>("right-choice-image");
        _leftStatusLabel = root.Q<Label>("left-status");
        _rightStatusLabel = root.Q<Label>("right-status");
        _leftNameLabel = root.Q<Label>("left-name");
        _rightNameLabel = root.Q<Label>("right-name");
        _leftActionLabel = root.Q<Label>("left-action");
        _rightActionLabel = root.Q<Label>("right-action");

        return true;
    }

    /// <summary>
    /// Sets the master container to be visible and updates its opacity context.
    /// </summary>
    private void ShowContainer()
    {
        if (_container == null) return;
        _container.style.display = DisplayStyle.Flex;
        _container.style.opacity = 1f;
    }

    /// <summary>
    /// Binds data models into the corresponding text labels and looks up the required action sprite artwork.
    /// </summary>
    /// <param name="player">The player model state containing game parameters.</param>
    /// <param name="isLeft">Determines if the target UI components belong to the left or right side.</param>
    /// <returns>The resolved Sprite reference corresponding to the hand gesture.</returns>
    private Sprite ConfigurePlayerUI(PlayerInfoModel player, bool isLeft)
    {
        Label statusLabel = isLeft ? _leftStatusLabel : _rightStatusLabel;
        Label nameLabel = isLeft ? _leftNameLabel : _rightNameLabel;
        Label actionLabel = isLeft ? _leftActionLabel : _rightActionLabel;

        if (player == null)
        {
            Debug.LogError("[ChoiceReveal] Player info is missing.");
            SetLabelText(statusLabel, "UNKNOWN");
            SetLabelText(nameLabel, "UNKNOWN");
            SetLabelText(actionLabel, "UNKNOWN");
            return null;
        }

        bool isAttacking = player.attacking;
        SetLabelText(statusLabel, isAttacking ? "ATTACK" : "DEFENSE");
        SetLabelText(nameLabel, string.IsNullOrWhiteSpace(player.username) ? "UNKNOWN" : player.username);

        Sprite sprite = ResolveChoiceSprite(player, isAttacking, out string actionName);
        SetLabelText(actionLabel, actionName);
        return sprite;
    }

    /// <summary>
    /// Parses hand action strings and queries the ActionDatabase to retrieve matching descriptive labels and localized sprites.
    /// </summary>
    /// <param name="player">The source player model containing raw choices.</param>
    /// <param name="isAttacking">Determines whether to search inside Attack or Defense sub-databases.</param>
    /// <param name="actionName">Output parameter providing the legible name string for the resolved action.</param>
    /// <returns>The loaded resource Sprite instance, or null if database lookups fail.</returns>
    private Sprite ResolveChoiceSprite(PlayerInfoModel player, bool isAttacking, out string actionName)
    {
        actionName = "UNKNOWN";
        if (player == null)
        {
            return null;
        }

        if (!GameSetting.TryParseHandAction(player.handChoice, out HandActionType action))
        {
            Debug.LogError($"[ChoiceReveal] Unknown handChoice: {player.handChoice}");
            return null;
        }

        List<HandActionData> actions = isAttacking ? ActionDatabase.AttackActions : ActionDatabase.DefendActions;
        HandActionData actionData = actions.Find(data => data.actionCode == action);
        if (actionData == null)
        {
            Debug.LogError($"[ChoiceReveal] Missing action data for {action} (attacking={isAttacking}).");
            return null;
        }

        actionName = actionData.actionName;
        Sprite sprite = ActionDatabase.GetActionSprite(actionData.imagePath);
        if (sprite == null)
        {
            Debug.LogError($"[ChoiceReveal] Missing sprite at Resources/{actionData.imagePath} for {actionData.actionName}.");
        }

        return sprite;
    }
    
    /// <summary>
    /// Safe utility wrapper to modify a dynamic text label's current display text value.
    /// </summary>
    /// <param name="label">The target UI Toolkit Label reference.</param>
    /// <param name="text">The string message to display.</param>
    private void SetLabelText(Label label, string text)
    {
        if (label == null) return;
        label.text = text;
    }
}