using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


//202322158 이준상

/// <summary>
/// Calls the functions in the class when Matchstate (called LobbyState in this project) is GAME_PLAYER_CHOICE.
/// </summary>
public class MainBattleActionRenderer
{
    private const int ActionScaleAnimationMs = 300;

    
    //hand action data
    private readonly VisualTreeAsset _actionItemSelect;
    private readonly List<VisualElement> _actionElements = new();
    private readonly Action<HandActionType, String> _onActionClicked;

    public MainBattleActionRenderer(VisualTreeAsset actionItemSelect, Action<HandActionType, String> onActionClicked = null)
    {
        _actionItemSelect = actionItemSelect;
        _onActionClicked = onActionClicked;
    }

    
    /// <summary>
    /// This function is executed when the callback function in the MainBattleBindingRender is executed.
    /// I created UI and UI animation code with AI.
    /// </summary>
    /// <param name="container"></param>
    /// <param name="isAttacker"></param>
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
        
        container.style.flexDirection = FlexDirection.Row;   
        container.style.justifyContent = Justify.Center;       
        container.style.alignItems = Align.FlexEnd;         
        container.style.width = Length.Percent(100);       
        container.style.height = Length.Auto();
        
        _actionElements.Clear();

        //Get HandActionData
        List<HandActionData> handActionDatas = isAttacker ? ActionDatabase.AttackActions : ActionDatabase.DefendActions;
        int actionCount = Mathf.Min(GameSetting.ATTACK, handActionDatas.Count);

        for (int i = 0; i < actionCount; i++)
        {
            var item = _actionItemSelect.Instantiate();
            item.style.width = 100; 
            item.style.height = 100;
            
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
                card.RegisterCallback<ClickEvent>(_ => OnActionClicked(actionCode, actionData.actionName));
            }

            item.schedule.Execute(() => item.style.scale = new StyleScale(Vector3.one)).StartingIn(50);
        }
    }

    /// <summary>
    /// This function is executed when the callback function in the MainBattleBindingRender is executed.
    /// </summary>
    /// <param name="container"></param>
    /// <param name="isAttacker"></param>
    public void HideAllActionOptions()
    {
        foreach (VisualElement option in _actionElements)
        {
            option.pickingMode = PickingMode.Ignore;
            option.style.scale = new StyleScale(Vector3.zero);
        }
    }

    /// <summary>
    /// When Player choose one of five actions, the Action will be invoked.
    /// </summary>
    /// <param name="actionType"></param>
    /// <param name="actionDataActionName"></param>
    private void OnActionClicked(HandActionType actionType, string actionDataActionName)
    {
        _onActionClicked?.Invoke(actionType, actionDataActionName);
    }
}
