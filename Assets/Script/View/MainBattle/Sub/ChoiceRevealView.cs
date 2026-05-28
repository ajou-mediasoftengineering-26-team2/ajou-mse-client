using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ChoiceRevealView : MonoBehaviour
{
    private VisualElement _container; // 전체를 감싸는 컨테이너 저장
    private VisualElement _leftPlayerGroup;
    private VisualElement _rightPlayerGroup;

    private VisualElement _leftChoiceImage;
    private VisualElement _rightChoiceImage;

    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        // 0. 컨테이너 가져오기
        _container = root.Q<VisualElement>(className: "display-container");

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

        // 💡 오브젝트가 켜질 때 UI를 완전히 깨끗한 초기 상태로 만듭니다.
        ResetAllStates();
    }

    public void StartChoiceReveal()
    {
        // 실제 사용 시Resources.Find... 대신 외부에서 스프라이트를 넘겨주는 게 좋습니다.
        var sprites = Resources.FindObjectsOfTypeAll<Sprite>();
        if (sprites.Length >= 2)
        {
            RevealChoices(sprites[0], sprites[1]);
        }
    }

    // 💡 모든 상태를 처음 상태로 되돌리는 통합 리셋 함수
    private void ResetAllStates()
    {
        if (_container != null)
        {
            _container.style.opacity = 1f; // 컨테이너 다시 보이게
        }

        InitInitialState(_leftPlayerGroup);
        InitInitialState(_rightPlayerGroup);
        
        // 보더 색상도 초기화하고 싶다면 여기서 투명이나 기본색으로 리셋
        ResetBorders(_leftChoiceImage);
        ResetBorders(_rightChoiceImage);
    }

    private void InitInitialState(VisualElement element)
    {
        if (element == null) return;
        
        element.style.opacity = 0f;
        element.style.translate = new StyleTranslate(new Translate(0, 50, 0));
        
        element.style.transitionProperty = new List<StylePropertyName> { "opacity", "translate" };
        element.style.transitionDuration = new List<TimeValue> 
            { new TimeValue(0.5f, TimeUnit.Second), new TimeValue(0.5f, TimeUnit.Second) };
        element.style.transitionTimingFunction = new List<EasingFunction> 
            { new EasingFunction(EasingMode.EaseOutBack) };
    }

    private void ResetBorders(VisualElement image)
    {
        if (image == null) return;
        image.style.borderTopColor = StyleKeyword.Null;
        image.style.borderBottomColor = StyleKeyword.Null;
        image.style.borderLeftColor = StyleKeyword.Null;
        image.style.borderRightColor = StyleKeyword.Null;
    }

    public void RevealChoices(Sprite leftPlayerSprite, Sprite rightPlayerSprite)
    {
        // 실행하기 전에 한 번 더 확실하게 리셋 보장
        ResetAllStates();

        if (leftPlayerSprite != null && _leftChoiceImage != null)
            _leftChoiceImage.style.backgroundImage = new StyleBackground(leftPlayerSprite);

        if (rightPlayerSprite != null && _rightChoiceImage != null)
            _rightChoiceImage.style.backgroundImage = new StyleBackground(rightPlayerSprite);

        PlaySequenceAnimation();
    }

    private void PlaySequenceAnimation()
    {
        // ⚠️ 혹시 모를 이전 스케줄 제거를 위해 확실히 털고 가기 (선택사항)
        _leftPlayerGroup.panel.visualTree.schedule.Execute(() => { }).StartingIn(0);

        // --- Player 1 팝업 (2.0초 대기 후 등장) ---
        _leftPlayerGroup.schedule.Execute(() =>
        {
            _leftPlayerGroup.style.opacity = 1f;
            _leftPlayerGroup.style.translate = new StyleTranslate(new Translate(0, 0, 0));

            if (_leftChoiceImage != null)
            {
                Color color = new Color32(255, 200, 0, 255);
                _leftChoiceImage.style.borderTopColor = new StyleColor(color);
                _leftChoiceImage.style.borderBottomColor = new StyleColor(color);
                _leftChoiceImage.style.borderLeftColor = new StyleColor(color);
                _leftChoiceImage.style.borderRightColor = new StyleColor(color); // 원본 코드의 오타 수정 (_right->_left)
            }
        }).StartingIn(2000); 

        // --- Player 2 팝업 (3.5초 대기 후 등장 - 1번이 나오고 1.5초 뒤이므로 2000+1500) ---
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
        }).StartingIn(3500); // 💡 1번 컴포넌트 등장 시간과의 싱크를 위해 3500으로 수정

        // --- 6초 뒤에 전체 UI 숨기기 시작 (애니메이션 다 보고 여유있게 지우기) ---
        _leftPlayerGroup.schedule.Execute(CloseUI).StartingIn(6000);
    }

    private void CloseUI()
    {
        if (_container != null)
        {
            _container.style.transitionProperty = new List<StylePropertyName> { "opacity" };
            _container.style.transitionDuration = new List<TimeValue> { new TimeValue(0.5f, TimeUnit.Second) };
            _container.style.opacity = 0f;

            // 페이드 아웃 애니메이션(0.5초)이 완벽히 끝난 후 오브젝트를 끕니다.
            _container.schedule.Execute(() => 
            {
                gameObject.SetActive(false);
            }).StartingIn(500);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}