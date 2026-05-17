using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


//202322158 이준상
public class MainBattleActionRenderer
{
    private const int ActionScaleAnimationMs = 300;

    private readonly VisualTreeAsset _actionItemSelect;
    private readonly List<VisualElement> _actionElements = new();
    private readonly Action<HandActionType> _onActionClicked;
    private bool _isActionAnimatingOut;

    public MainBattleActionRenderer(VisualTreeAsset actionItemSelect, Action<HandActionType> onActionClicked = null)
    {
        _actionItemSelect = actionItemSelect;
        _onActionClicked = onActionClicked;
    }

    public void ShowActions(VisualElement container, bool isAttacker)
    {
        if (container == null)
        {
            Debug.LogError("ShowActions failed: container is null.");
            return;
        }

        if (_actionItemSelect == null)
        {
            Debug.LogError("ShowActions failed: actionItemSelect is not assigned.");
            return;
        }

        container.Clear();
        _actionElements.Clear();
        _isActionAnimatingOut = false;

        List<HandActionData> handActionDatas = isAttacker ? ActionDatabase.AttackActions : ActionDatabase.DefendActions;
        int actionCount = Mathf.Min(GameSetting.ATTACK, handActionDatas.Count);

        for (int i = 0; i < actionCount; i++)
        {
            var item = _actionItemSelect.Instantiate();
            item.style.scale = new StyleScale(Vector3.zero);
            item.style.transitionProperty = new StyleList<StylePropertyName>(new List<StylePropertyName> { "scale" });
            item.style.transitionDuration = new StyleList<TimeValue>(new List<TimeValue> { ActionScaleAnimationMs / 1000f });
            item.style.transitionTimingFunction = new StyleList<EasingFunction>(new List<EasingFunction> { new EasingFunction(EasingMode.EaseOut) });
            container.Add(item);

            HandActionData actionData = handActionDatas[i];
            if (actionData == null)
            {
                Debug.LogWarning($"ShowActions: action data is null at index {i}.");
                continue;
            }

            var text = item.Q<Label>("ItemName");
            if (text != null) text.text = actionData.actionName;

            var iconImage = item.Q<VisualElement>("IconImage");
            if (iconImage != null)
            {
                iconImage.style.backgroundImage = new StyleBackground(ActionDatabase.GetActionSprite(actionData.imagePath));
            }

            _actionElements.Add(item);
            var card = item.Q<VisualElement>("CardContainer");
            if (card != null)
            {
                HandActionType actionCode = actionData.actionCode;
                card.RegisterCallback<ClickEvent>(_ => OnActionClicked(actionCode));
            }

            item.schedule.Execute(() => item.style.scale = new StyleScale(Vector3.one)).StartingIn(50);
        }
    }

    public void HideAllActionOptions()
    {
        foreach (VisualElement option in _actionElements)
        {
            option.pickingMode = PickingMode.Ignore;
            option.style.scale = new StyleScale(Vector3.zero);
        }
    }

    private void OnActionClicked(HandActionType actionType)
    {
        if (_isActionAnimatingOut) return;
        _isActionAnimatingOut = true;
        _onActionClicked?.Invoke(actionType);
    }
}
