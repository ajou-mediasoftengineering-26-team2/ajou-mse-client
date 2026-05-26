using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ChoiceRevealView : MonoBehaviour
{
    private VisualElement _leftPlayerGroup;
    private VisualElement _rightPlayerGroup;

    private VisualElement _leftChoiceImage;
    private VisualElement _rightChoiceImage;

    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        // 1. 컴포넌트 구조 잡기
        var playerGroups = root.Query<VisualElement>(className: "player-group").ToList();
        if (playerGroups.Count >= 2)
        {
            _leftPlayerGroup = playerGroups[0];
            _rightPlayerGroup = playerGroups[1];
        }

        // 2. Sprite가 들어갈 엘리먼트 가져오기
        _leftChoiceImage = root.Q<VisualElement>("left-choice-image");
        _rightChoiceImage = root.Q<VisualElement>("right-choice-image");

        // 3. 초기 상태 및 트랜지션 규칙 세팅 (여기서 완벽하게 정의)
        InitInitialState(_leftPlayerGroup);
        InitInitialState(_rightPlayerGroup);

        // 테스트용 (실제로는 외부에서 호출)
        
    }

    public void StartChoiceReveal()
    {
        RevealChoices(Resources.FindObjectsOfTypeAll<Sprite>()[0], Resources.FindObjectsOfTypeAll<Sprite>()[1]);
    }

    private void InitInitialState(VisualElement element)
    {
        if (element == null) return;
        
        // 시작 상태
        element.style.opacity = 0f;
        element.style.translate = new StyleTranslate(new Translate(0, 50, 0));
        
        // 💡 트랜지션 규칙은 여기서 딱 한 번만 지정합니다 (시간을 0.5초로 통일)
        element.style.transitionProperty = new List<StylePropertyName> { "opacity", "translate" };
        element.style.transitionDuration = new List<TimeValue> 
            { new TimeValue(0.5f, TimeUnit.Second), new TimeValue(0.5f, TimeUnit.Second) };
        element.style.transitionTimingFunction = new List<EasingFunction> 
            { new EasingFunction(EasingMode.EaseOutBack) };
    }

    public void RevealChoices(Sprite leftPlayerSprite, Sprite rightPlayerSprite)
    {
        if (leftPlayerSprite != null && _leftChoiceImage != null)
            _leftChoiceImage.style.backgroundImage = new StyleBackground(leftPlayerSprite);

        if (rightPlayerSprite != null && _rightChoiceImage != null)
            _rightChoiceImage.style.backgroundImage = new StyleBackground(rightPlayerSprite);

        PlaySequenceAnimation();
    }

    private void PlaySequenceAnimation()
    {
        // --- Player 1 팝업 (0.5초 대기 후 등장) ---
        _leftPlayerGroup.schedule.Execute(() =>
        {
            // 💡 여기서는 값만 바꿉니다. UI Toolkit이 알아서 인지하고 부드럽게 연출합니다.
            _leftPlayerGroup.style.opacity = 1f;
            _leftPlayerGroup.style.translate = new StyleTranslate(new Translate(0, 0, 0));

            if (_leftChoiceImage != null)
            {
                Color color = new Color32(255, 200, 0, 255);
                _leftChoiceImage.style.borderTopColor = new StyleColor(color);
                _leftChoiceImage.style.borderBottomColor = new StyleColor(color);
                _leftChoiceImage.style.borderLeftColor = new StyleColor(color);
                _rightChoiceImage.style.borderRightColor = new StyleColor(color);
            }
        }).StartingIn(2000); 

        // --- Player 2 팝업 (1.5초 대기 후 등장) ---
        _rightPlayerGroup.schedule.Execute(() =>
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
        }).StartingIn(1500); 

        // --- 4.5초 뒤에 전체 UI 숨기기 ---
        _leftPlayerGroup.schedule.Execute(CloseUI).StartingIn(4500);
    }

    private void CloseUI()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        var container = root.Q<VisualElement>(className: "display-container");
        if (container != null)
        {
            container.style.transitionProperty = new List<StylePropertyName> { "opacity" };
            container.style.transitionDuration = new List<TimeValue> { new TimeValue(0.5f, TimeUnit.Second) };
            container.style.opacity = 0f;

            container.schedule.Execute(() => gameObject.SetActive(false)).StartingIn(500);
        }
        
        
        InitInitialState(_leftPlayerGroup);
        InitInitialState(_rightPlayerGroup);
    }
}