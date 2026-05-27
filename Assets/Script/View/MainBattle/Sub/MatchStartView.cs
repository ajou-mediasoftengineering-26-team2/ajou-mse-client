using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MatchStartView : MonoBehaviour
{
    private VisualElement _leftPlayerGroup;
    private VisualElement _rightPlayerGroup;
    
    
    void OnEnable()
    {
        var uiDoc = GetComponent<UIDocument>();
        if (uiDoc == null) return;

        var root = uiDoc.rootVisualElement;

        // 1. UXML에서 플레이어 그룹 2개 찾아오기
        var playerGroups = root.Query<VisualElement>(className: "player-group").ToList();
        if (playerGroups.Count >= 2)
        {
            _leftPlayerGroup = playerGroups[0];
            _rightPlayerGroup = playerGroups[1];
        }

        
    }

    public void StartAnimation()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        // 2. 초기 상태 셋팅 (화면 아래에 숨겨두기)
        InitInitialState(_leftPlayerGroup);
        InitInitialState(_rightPlayerGroup);

        // 테스트: 1초 뒤에 순차 팝업 애니메이션 실행
        root.schedule.Execute(PlaySequenceAnimation).StartingIn(1000);
    }

    /// <summary>
    /// 요소의 초기 위치(아래로 50px)와 투명도(0), 그리고 부드러운 연출을 위한 트랜지션을 코드로 주입합니다.
    /// </summary>
    private void InitInitialState(VisualElement element)
    {
        if (element == null) return;

        // 시작 상태: 투명하고 아래로 50px 내려감
        element.style.opacity = 0f;
        element.style.translate = new StyleTranslate(new Translate(0, 50, 0));

        // 코드로 트랜지션(부드러운 움직임) 속성 심기
        element.style.transitionProperty = new List<StylePropertyName> { "opacity", "translate" };
        element.style.transitionDuration = new List<TimeValue> 
        { 
            new TimeValue(0.4f, TimeUnit.Second), 
            new TimeValue(0.4f, TimeUnit.Second) 
        };
        
        // 가속도 운동 규칙 지정 (약간 통통 튀는 이징 함수 효과 코드)
        element.style.transitionTimingFunction = new List<EasingFunction> 
        { 
            new EasingFunction(EasingMode.EaseOutBack) 
        };
    }

    /// <summary>
    /// 순수 C# 스케줄러로 제어하는 릴레이 팝업 애니메이션
    /// </summary>
    public void PlaySequenceAnimation()
    {
        if (_leftPlayerGroup == null || _rightPlayerGroup == null) return;
        
        _leftPlayerGroup.schedule.Execute(() =>
        {
            _leftPlayerGroup.style.opacity = 1f;
            _leftPlayerGroup.style.translate = new StyleTranslate(new Translate(0, 0, 0));
            Debug.Log("Player 1 애니메이션 시작");
        }).StartingIn(50); // 🚨 핵심: 유니티가 초기 설정을 인지할 수 있도록 0.05초 딜레이를 줍니다.


        _rightPlayerGroup.schedule.Execute(() =>
        {
            _rightPlayerGroup.style.opacity = 1f;
            _rightPlayerGroup.style.translate = new StyleTranslate(new Translate(0, 0, 0));
            Debug.Log("Player 2 애니메이션 시작");
        }).StartingIn(550); 
        
        _leftPlayerGroup.schedule.Execute(() =>
        {
            CloseScoreUI();
        }).StartingIn(3550);
    }

    private void CloseScoreUI()
    {
        Debug.Log("3초 대기 완료 - UI 종료 시작");

        // [선택 1] 부드럽게 페이드 아웃하면서 사라지게 하고 싶을 때
        _leftPlayerGroup.style.opacity = 0f;
        _rightPlayerGroup.style.opacity = 0f;

        // 만약 전체 전광판 배경(display-container)도 같이 끄고 싶다면 아래처럼 루트 레이아웃을 건드려도 됩니다.
        var root = GetComponent<UIDocument>().rootVisualElement;
        var displayContainer = root.Q<VisualElement>(className: "display-container");

        if (displayContainer != null)
        {
            // 부드럽게 사라지도록 트랜지션 부여 후 투명화
            displayContainer.style.transitionProperty = new List<StylePropertyName> { "opacity" };
            displayContainer.style.transitionDuration = new List<TimeValue> { new TimeValue(0.5f, TimeUnit.Second) };
            displayContainer.style.opacity = 0f;

            // 완전히 투명해진 0.5초(500ms) 뒤에 UI를 화면에서 아예 안 보이게 처리(Remove 또는 display)
            displayContainer.schedule.Execute(() =>
            {
                displayContainer.style.display = DisplayStyle.None; // 혹은 게임 오브젝트 자체를 끄기: gameObject.SetActive(false);
                Debug.Log("UI 종료 완료");
            }).StartingIn(500);
        }
        
        
    }
}
