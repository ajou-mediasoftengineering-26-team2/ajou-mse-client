
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework.Constraints;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

//202322158 이준상


/// <summary>
/// View class responsible for main battle UI visuals and data binding with the ViewModel.
/// I don't mean to, but I know all too well that there's a situation where one class learns everything and that's why it's inconvenient to expand the code later on.
/// So, I will be split the code into components.
/// </summary>
public class MainBattleView : MonoBehaviour
{
    private const int ActionScaleAnimationMs = 300;
    private bool _isActionAnimatingOut;
    private MainBattleViewModel _viewModel;
    
    private VisualElement _myRoundWining;
    private VisualElement _enemyRoundWining;

    private VisualElement _actionElement;
    
    private VisualElement _mainBattleRoot;
    private VisualElement _perksRoot;
    private Label _timer;
    
    private readonly List<VisualElement> _myDotElements = new();
    private readonly List<VisualElement> _enemyDotElements = new();
    private readonly List<VisualElement> _actionElements = new();
    [SerializeField]
    public UIDocument mainBattle;
    public UIDocument perks;
    public VisualTreeAsset roundItemTemplate;
    public VisualTreeAsset actionItemSelect;
    private void OnEnable()
    {
        
        _mainBattleRoot = mainBattle.rootVisualElement; 
        _perksRoot = perks.rootVisualElement;
        
        _myRoundWining = _mainBattleRoot.Q<VisualElement>("MyRoundContainer");
        _enemyRoundWining = _mainBattleRoot.Q<VisualElement>("EnemyRoundContainer");
        _actionElement = _mainBattleRoot.Q<VisualElement>("ChooseAction");
        _timer = _mainBattleRoot.Q<Label>("Time");
        viewModelSetting();
        
        InitializeDots(_myRoundWining, _myDotElements);
        InitializeDots(_enemyRoundWining, _enemyDotElements);
    }

    /// <summary>
    /// ViewModel, Observer setting
    /// </summary>
    private void viewModelSetting()
    {
        _viewModel = ViewModelLocator.Instance.Get<MainBattleViewModel>();
    
        _viewModel.SetPlayerAndMatchId(SceneDataBridge.playerId,  SceneDataBridge.MatchId, SceneDataBridge.enemyId);
        
        //If round win, dot ui will be change
        _viewModel.LeftRoundWin.Subscribe(val => RefreshDots(_myDotElements, val));
        _viewModel.RightRoundWin.Subscribe(val => RefreshDots(_enemyDotElements, val));
        _viewModel.StationName.Subscribe(station =>
        {
            var label = _mainBattleRoot.Q<Label>("CurrentStation");
            label.text = station;
        });
        
        _viewModel.MySelectingE.Subscribe(selecting =>
        {
            var indicator = _mainBattleRoot.Q<VisualElement>("TurnIndicator");
            
            // Class control: Add class if the second factor is true; remove if false
            indicator.EnableInClassList("my-turn", _viewModel.MySelecting.Value);
            indicator.EnableInClassList("enemy-turn", !_viewModel.MySelecting.Value);
            
            Debug.Log(selecting + " selecting value *********************");
            
            //Only show action card when my turn.
            System.Action action = _viewModel.MySelecting.Value ? () => UpdateRoundWithDelay() : () => HideAllActionOptions(_actionElements);
            action();
        });
        // _viewModel.MySelecting.Subscribe(selecting =>
        // {
        //     var indicator = _mainBattleRoot.Q<VisualElement>("TurnIndicator");
        //     
        //     // Class control: Add class if the second factor is true; remove if false
        //     indicator.EnableInClassList("my-turn", _viewModel.MySelecting.Value);
        //     indicator.EnableInClassList("enemy-turn", !_viewModel.MySelecting.Value);
        //     
        //     Debug.Log(selecting + " selecting value *********************");
        //     
        //     //Only show action card when my turn.
        //     System.Action action = _viewModel.MySelecting.Value ? () => UpdateRoundWithDelay() : () => HideAllActionOptions(_actionElements);
        //     action();
        // });
        //
        // _viewModel.IsAttacker.Subscribe(selecting =>
        // {
        //     var indicator = _mainBattleRoot.Q<VisualElement>("TurnIndicator");
        //     
        //     // Class control: Add class if the second factor is true; remove if false
        //     indicator.EnableInClassList("my-turn", _viewModel.MySelecting.Value);
        //     indicator.EnableInClassList("enemy-turn", !_viewModel.MySelecting.Value);
        //     
        //     Debug.Log(selecting + " selecting value *********************");
        //     
        //     //Only show action card when my turn.
        //     System.Action action = _viewModel.MySelecting.Value ? () => UpdateRoundWithDelay() : () => HideAllActionOptions(_actionElements);
        //     action();
        // });
        
        //Setting current matchState ex) Your turn, Enemy turn...
        _viewModel.LabelState.Subscribe(labelText =>
        {
            var label = _mainBattleRoot.Q<Label>("TurnText");

            label.text = _viewModel.LabelState.Value;
        });
        
        //My Hp
        _viewModel.LeftHp.Subscribe(myHp =>
        {
            var hpFill = _mainBattleRoot.Q<VisualElement>("MyHPFill");
            float targetRatio = Mathf.Clamp01((float)myHp / GameSetting.maxHP);
            // Use % operation
            hpFill.style.width = new Length(targetRatio * 100, LengthUnit.Percent);
        });

        //Enemy Hp
        _viewModel.RightHp.Subscribe(enemyHp =>
        {
            var hpFill = _mainBattleRoot.Q<VisualElement>("EnemyHPFill");
            float targetRatio = Mathf.Clamp01((float)enemyHp / GameSetting.maxHP);
            // Use % operation
            hpFill.style.width = new Length(targetRatio * 100, LengthUnit.Percent);
        });
        
        _viewModel.CountDown.Subscribe(time =>
        {
            _timer.text = time;
        });
    }

    public async void UpdateRoundWithDelay()
    {
        ChooseAction(_actionElement, _actionElements);
    }
    
    
    /// <summary>
    /// init dot UI
    /// </summary>
    /// <param name="container"></param>
    /// <param name="cacheList"></param>
    private void InitializeDots(VisualElement container, List<VisualElement> cacheList)
    {
        container.Clear();
        cacheList.Clear();
    
        for (int i = 0; i < GameSetting.ROUNDWINING; i++)
        {
            var item = roundItemTemplate.Instantiate();
            item.style.flexDirection = FlexDirection.Row;
            item.style.marginRight = 5;
            item.style.marginLeft = 5;
            container.Add(item);
    
            var dot = item.Q<VisualElement>("Dot");
            if (dot != null) cacheList.Add(dot);
        }
    }
    
    /// <summary>
    /// Show Action Card UI
    /// </summary>
    /// <param name="container"></param>
    /// <param name="cacheList"></param>
    private void ChooseAction(VisualElement container, List<VisualElement> cacheList)
    {
        if (container == null)
        {
            Debug.LogError("ChooseAction: container is null.");
            return;
        }
    
        //clear Action element
        container.Clear();
        cacheList.Clear();
        _isActionAnimatingOut = false;

        // Get Hand Action data
        List<HandActionData> handActionDatas =
            _viewModel.IsAttacker.Value ? ActionDatabase.AttackActions : ActionDatabase.DefendActions;
    
        Debug.Log(handActionDatas[0].actionName + "when action ready &&&&&&&");
        if (actionItemSelect == null)
        {
            Debug.LogError("ChooseAction: actionItemSelect is not assigned.");
            return;
        }
    
        int actionCount = Mathf.Min(GameSetting.ATTACK, handActionDatas.Count);
        for (int i = 0; i < actionCount; i++)
        {
            var item = actionItemSelect.Instantiate();
    
            // init
            item.style.scale = new StyleScale(Vector3.zero); // 시작은 0
    
            // Animation Setting Code
            item.style.transitionProperty = new StyleList<StylePropertyName>(new List<StylePropertyName> { "scale" });
            item.style.transitionDuration = new StyleList<TimeValue>(new List<TimeValue> { ActionScaleAnimationMs / 1000f });
            item.style.transitionTimingFunction = new StyleList<EasingFunction>(new List<EasingFunction> { new EasingFunction(EasingMode.EaseOut) });
    
            container.Add(item);
    
            //Item Setting Code
            HandActionData actionData = handActionDatas[i];
            if (actionData == null)
            {
                Debug.LogWarning($"ChooseAction: action data is null at index {i}.");
                continue;
            }
    
            var text = item.Q<Label>("ItemName");
            if (text != null)
            {
                text.text = actionData.actionName;
            }
    
            VisualElement iconImage = item.Q<VisualElement>("IconImage");
            if (iconImage != null)
            {
                iconImage.style.backgroundImage = new StyleBackground(ActionDatabase.GetActionSprite(actionData.imagePath));
            }
    
            //Add Element on Scene
            cacheList.Add(item);
            var card = item.Q<VisualElement>("CardContainer");
            if (card != null)
            {
                HandActionType actionCode = actionData.actionCode; 
                card.RegisterCallback<ClickEvent>(_ =>
                    OnActionClicked(actionCode));
            }
            //animation Execute
            item.schedule.Execute(() =>
            {
                item.style.scale = new StyleScale(Vector3.one);
            }).StartingIn(50);
        }
    }
    
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="actionIndex"></param>
    private void OnActionClicked(HandActionType actionIndex)
    {
        if (_isActionAnimatingOut) return;
        _isActionAnimatingOut = true;

        HideAllActionOptions(_actionElements);
        Debug.Log($"Action clicked: {actionIndex}");
        // 여기서 ViewModel 호출 / 서버 전송 / EventBus 발행
        _viewModel.OnHandAction(actionIndex);
        EventBus.Publish(new ActionSelectedEvent(actionIndex));
    }

    private static void HideAllActionOptions(IEnumerable<VisualElement> options)
    {
        foreach (VisualElement option in options)
        {
            option.pickingMode = PickingMode.Ignore;
            option.style.scale = new StyleScale(Vector3.zero);
        }
    }
    
    private void RefreshDots(List<VisualElement> dots, int currentWins)
    {
        Debug.Log(currentWins + "how?");
        for (int i = 0; i < dots.Count; i++)
        {
            dots[i].EnableInClassList("round-dot--active", i < currentWins);
        }
    }
    // viewmodel dispose
    private void OnDestroy()
    {
        _viewModel?.Dispose();
    }
}
