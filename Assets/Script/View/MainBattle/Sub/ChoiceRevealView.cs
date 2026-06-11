using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

//202322158 이준상

/// <summary>
/// UI class showing the hand action you and the other person chose All animation codes are produced by AI.
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

    void OnEnable()
    {
        TryCacheElements();

        SnapToInitialState();
    }

    void OnDisable()
    {
        if (_animationCoroutine != null)
        {
            StopCoroutine(_animationCoroutine);
            _animationCoroutine = null;
        }
    }

    /// <summary>
    /// 외부 테스트용 메서드 (무작위 스프라이트 트랩 방지 로그 추가)
    /// </summary>
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

    private void SnapToInitialState()
    {
        if (_container != null)
        {
            _container.style.transitionProperty = StyleKeyword.Null; // 트랜지션 일시 제거
            _container.style.opacity = 1f; // 컨테이너는 항상 보이게
        }

        SnapElementZero(_leftPlayerGroup);
        SnapElementZero(_rightPlayerGroup);
        ResetBorders(_leftChoiceImage);
        ResetBorders(_rightChoiceImage);
    }

    private void SnapElementZero(VisualElement element)
    {
        if (element == null) return;
        element.style.transitionProperty = StyleKeyword.Null; // 트랜지션 없이 즉시 반영하기 위함
        element.style.opacity = 0f;
        element.style.translate = new StyleTranslate(new Translate(0, 50, 0));
    }

    private void ResetBorders(VisualElement image)
    {
        if (image == null) return;
        image.style.borderTopColor = StyleKeyword.Null;
        image.style.borderBottomColor = StyleKeyword.Null;
        image.style.borderLeftColor = StyleKeyword.Null;
        image.style.borderRightColor = StyleKeyword.Null;
    }

    /// <summary>
    /// 실제 애니메이션을 트리거하는 메인 진입점
    /// </summary>
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

    private void ApplyTransitionRules(VisualElement element)
    {
        if (element == null) return;
        element.style.transitionProperty = new List<StylePropertyName> { "opacity", "translate" };
        element.style.transitionDuration = new List<TimeValue> 
            { new TimeValue(0.5f, TimeUnit.Second), new TimeValue(0.5f, TimeUnit.Second) };
        element.style.transitionTimingFunction = new List<EasingFunction> 
            { new EasingFunction(EasingMode.EaseOutBack) };
    }

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

    private void ShowContainer()
    {
        if (_container == null) return;
        _container.style.display = DisplayStyle.Flex;
        _container.style.opacity = 1f;
    }

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

    private void SetLabelText(Label label, string text)
    {
        if (label == null) return;
        label.text = text;
    }
}