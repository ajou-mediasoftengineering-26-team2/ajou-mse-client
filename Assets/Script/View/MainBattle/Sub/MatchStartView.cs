using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

//202322158


/// <summary>
/// When Match is stared, This UI will be enabled
/// </summary>
public class MatchStartView : MonoBehaviour
{
    private VisualElement _leftPlayerGroup;
    private VisualElement _rightPlayerGroup;
    private VisualElement _displayContainer;
    
    private Label name1;
    private Label name2;
    private Label position1;
    private Label position2;

    private MainBattleViewModel _viewModel;

    void OnEnable()
    {
        _viewModel = ViewModelLocator.Instance.Get<MainBattleViewModel>();
        var uiDoc = GetComponent<UIDocument>();
        if (uiDoc == null) return;

        CacheElements(uiDoc.rootVisualElement);
    }

    /// <summary>
    /// start animation funciton
    /// </summary>
    /// <param name="player1"></param>
    /// <param name="player2"></param>
    public void StartAnimation(PlayerInfoModel player1, PlayerInfoModel player2)
    {

        Debug.Log("startAnimation 애니메이션 시작");
        if (player1 == null || player2 == null)
        {
            Debug.LogError("[MatchStartView] Player info is null. StartAnimation aborted.");
            return;
        }

        var uiDoc = GetComponent<UIDocument>();
        if (uiDoc == null)
        {
            Debug.LogError("[MatchStartView] UIDocument component is missing.");
            return;
        }

        var root = uiDoc.rootVisualElement;
        if (root == null)
        {
            Debug.LogError("[MatchStartView] Root VisualElement is null.");
            return;
        }

        CacheElements(root);

        if (_displayContainer != null)
        {
            _displayContainer.style.display = DisplayStyle.Flex;
            _displayContainer.style.opacity = 1f;
        }
        name1.text = player1.username;
        name2.text = player2.username;
        position1.text = player1.attacking ? "Attack" : "Defend";
        position2.text = player2.attacking ? "Attack" : "Defend";
        InitInitialState(_leftPlayerGroup);
        InitInitialState(_rightPlayerGroup);


        root.schedule.Execute(PlaySequenceAnimation).StartingIn(1000);
    }

    
    /// <summary>
    /// After the UI appeared only once,
    /// there was an error that did not appear after that.
    /// As a result of Googling with llm, they said that the UI cache should be erased, and I wrote the code.
    /// </summary>
    /// <param name="root"></param>
    private void CacheElements(VisualElement root)
    {
        if (root == null) return;

        _displayContainer = root.Q<VisualElement>(className: "display-container");


        var playerGroups = root.Query<VisualElement>(className: "player-group").ToList();
        _leftPlayerGroup = playerGroups.Count > 0 ? playerGroups[0] : null;
        _rightPlayerGroup = playerGroups.Count > 1 ? playerGroups[1] : null;

        name1 = root.Q<Label>("left-name");
        name2 = root.Q<Label>("right-name");
        position1 = root.Q<Label>("left-status");
        position2 = root.Q<Label>("right-status");
    }

    /// <summary>
    /// UI/UI animation. I wrote this code with AI.
    /// </summary>
    private void InitInitialState(VisualElement element)
    {
        if (element == null) return;

        element.style.opacity = 0f;
        element.style.translate = new StyleTranslate(new Translate(0, 50, 0));

        element.style.transitionProperty = new List<StylePropertyName> { "opacity", "translate" };
        element.style.transitionDuration = new List<TimeValue>
        {
            new TimeValue(0.4f, TimeUnit.Second),
            new TimeValue(0.4f, TimeUnit.Second)
        };

        element.style.transitionTimingFunction = new List<EasingFunction>
        {
            new EasingFunction(EasingMode.EaseOutBack)
        };
    }

    /// <summary>
    /// Relay pop-up animation controlled by pure C# scheduler
    /// Some of code created by AI
    /// </summary>
    public void PlaySequenceAnimation()
    {
        if (_leftPlayerGroup == null || _rightPlayerGroup == null) return;

        _leftPlayerGroup.schedule.Execute(() =>
        {
            _leftPlayerGroup.style.opacity = 1f;
            _leftPlayerGroup.style.translate = new StyleTranslate(new Translate(0, 0, 0));
        }).StartingIn(50);


        _rightPlayerGroup.schedule.Execute(() =>
        {
            _rightPlayerGroup.style.opacity = 1f;
            _rightPlayerGroup.style.translate = new StyleTranslate(new Translate(0, 0, 0));
        }).StartingIn(550);

        _leftPlayerGroup.schedule.Execute(() => { CloseScoreUI(); }).StartingIn(3550);
    }

    /// <summary>
    /// CloseScoreUI
    /// </summary>
    private void CloseScoreUI()
    {

        _leftPlayerGroup.style.opacity = 0f;
        _rightPlayerGroup.style.opacity = 0f;

        var root = GetComponent<UIDocument>().rootVisualElement;
        var displayContainer = root.Q<VisualElement>(className: "display-container");

        if (displayContainer != null)
        {
            displayContainer.style.transitionProperty = new List<StylePropertyName> { "opacity" };
            displayContainer.style.transitionDuration =
                new List<TimeValue> { new TimeValue(0.5f, TimeUnit.Second) };
            displayContainer.style.opacity = 0f;

            displayContainer.schedule.Execute(() =>
            {
                displayContainer.style.display =
                    DisplayStyle.None; // 혹은 게임 오브젝트 자체를 끄기: gameObject.SetActive(false);
            }).StartingIn(500);
        }


        _viewModel.PutRoundStartAck();
    }
}