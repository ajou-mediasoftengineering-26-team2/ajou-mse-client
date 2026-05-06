
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
public class MainBattleView : MonoBehaviour
{
    private const int ActionScaleAnimationMs = 300;
    private bool _isActionAnimatingOut;
    private MainBattleViewModel _viewModel;
    //뷰모델 참조
    private VisualElement myRoundWining;
    private VisualElement enemyRoundWining;

    private VisualElement actionElement;
    
    private VisualElement mainBattleRoot;
    private VisualElement perksRoot;
    
    
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
        
        mainBattleRoot = mainBattle.rootVisualElement; 
        perksRoot = perks.rootVisualElement;
        
        myRoundWining = mainBattleRoot.Q<VisualElement>("MyRoundContainer");
        enemyRoundWining = mainBattleRoot.Q<VisualElement>("EnemyRoundContainer");
        actionElement = mainBattleRoot.Q<VisualElement>("ChooseAction");
    
        InitializeDots(myRoundWining, _myDotElements);
        InitializeDots(enemyRoundWining, _enemyDotElements);
        viewModelSetting();
    }

    private void viewModelSetting()
    {
        _viewModel = ViewModelLocator.Instance.Get<MainBattleViewModel>();
    
        _viewModel.setPlayerAndMatchId(SceneDataBridge.playerId,  SceneDataBridge.MatchId, SceneDataBridge.enemyId);
        
        _viewModel.LeftRoundWin.Subscribe(val => RefreshDots(_myDotElements, val));
        _viewModel.RightRoundWin.Subscribe(val => RefreshDots(_enemyDotElements, val));
        _viewModel.currentTurn.Subscribe(selecting =>
        {
            var indicator = mainBattleRoot.Q<VisualElement>("TurnIndicator");
            
            // 클래스 제어: 두 번째 인자가 true면 클래스 추가, false면 제거됨
            indicator.EnableInClassList("my-turn", _viewModel.mySelecting.Value);
            indicator.EnableInClassList("enemy-turn", !_viewModel.mySelecting.Value);
            
            Debug.Log(selecting + " selecting value *********************");
            
            System.Action action = _viewModel.mySelecting.Value ? () => UpdateRoundWithDelay() : () => HideAllActionOptions(_actionElements);
            action();
        });
        
        _viewModel.labelState.Subscribe(labelText =>
        {
            var label = mainBattleRoot.Q<Label>("TurnText");

            label.text = _viewModel.labelState.Value;
        });
        
        _viewModel.LeftHp.Subscribe(myHp =>
        {
            var hpFill = mainBattleRoot.Q<VisualElement>("MyHPFill");
            float targetRatio = Mathf.Clamp01((float)myHp / GameSetting.maxHP);
            // width를 %로 직접 꽂아줌 (애니메이션 없이 즉시 반영)
            hpFill.style.width = new Length(targetRatio * 100, LengthUnit.Percent);
        });

        _viewModel.RightHp.Subscribe(enemyHp =>
        {
            var hpFill = mainBattleRoot.Q<VisualElement>("EnemyHPFill");
            float targetRatio = Mathf.Clamp01((float)enemyHp / GameSetting.maxHP);
            // width를 %로 직접 꽂아줌
            hpFill.style.width = new Length(targetRatio * 100, LengthUnit.Percent);
        });
    }

    public async void UpdateRoundWithDelay()
    {
        ChooseAction(actionElement, _actionElements);
    }
    
    
    private void InitializeDots(VisualElement container, List<VisualElement> cacheList)
    {
        container.Clear();
        cacheList.Clear();
    
        //원래 for문 쓰시면 안됩니다.
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
    
    private void ChooseAction(VisualElement container, List<VisualElement> cacheList)
    {
        if (container == null)
        {
            Debug.LogError("ChooseAction: container is null.");
            return;
        }
    
        container.Clear();
        cacheList.Clear();
        _isActionAnimatingOut = false;

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
    
            // 1. 초기화: 애니메이션 속성부터 먼저 정의
            item.style.scale = new StyleScale(Vector3.zero); // 시작은 0
    
            // 어떤 속성을(scale), 얼마동안(0.3s), 어떻게(EaseOut) 바꿀지 세트로 설정
            item.style.transitionProperty = new StyleList<StylePropertyName>(new List<StylePropertyName> { "scale" });
            item.style.transitionDuration = new StyleList<TimeValue>(new List<TimeValue> { ActionScaleAnimationMs / 1000f });
            item.style.transitionTimingFunction = new StyleList<EasingFunction>(new List<EasingFunction> { new EasingFunction(EasingMode.EaseOut) });
    
            container.Add(item);
    
            //아이템 세팅코드
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
    
            cacheList.Add(item);
            //추가된 직후 스케일을 1로 변경 (애니메이션 시작) -
            // container에 추가된 후, 유니티가 UI를 다시 그리는 다음 프레임에 실행되도록 스케줄러를 씁니다.
            // 이 한 줄로 스케일이 0에서 1로 부드럽게 커집니다.
            var card = item.Q<VisualElement>("CardContainer");
            if (card != null)
            {
                HandActionType actionCode = actionData.actionCode; 
                card.RegisterCallback<ClickEvent>(_ =>
                    OnActionClicked(actionCode));
            }
            item.schedule.Execute(() =>
            {
                item.style.scale = new StyleScale(Vector3.one);
            }).StartingIn(50);
        }
    }
    
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
        //여기도 삼항 연산자. 연산자만 view에서 허용합니다. for문은....list뷰를 써야되는데 도저히 방법을 못찾아서 일단 이 방법을 쓰겠습니다.
        Debug.Log(currentWins + "how?");
        for (int i = 0; i < dots.Count; i++)
        {
            dots[i].EnableInClassList("round-dot--active", i < currentWins);
        }
    }
    // 메모리 누수 방지를 위해 할당 해제
    private void OnDestroy()
    {
        _viewModel?.Dispose();
    }
}
